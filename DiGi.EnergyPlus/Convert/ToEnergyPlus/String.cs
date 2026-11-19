using DiGi.Analytical.Building;
using DiGi.Analytical.Building.Classes;
using DiGi.Analytical.Building.Energy.Classes;
using DiGi.Analytical.Building.Enums;
using DiGi.Analytical.Building.Interfaces;
using DiGi.Analytical.Building.Regulation;
using DiGi.Analytical.Building.Regulation.Classes;
using DiGi.Analytical.Building.Regulation.Enums;
using DiGi.Analytical.Classes;
using DiGi.Analytical.Interfaces;
using DiGi.Core.Classes;
using DiGi.Core.Parameter.Classes;
using DiGi.EPW.Classes;
using DiGi.Geometry.Core.Enums;
using DiGi.Geometry.Spatial.Classes;
using DiGi.Geometry.Spatial.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DiGi.EnergyPlus
{
    public static partial class Convert
    {
        /// <summary>
        /// Writes a prepared building model as an EnergyPlus input file (IDF) for <see cref="Constants.Default.Version"/>, so that EnergyPlus calculates the heating need of exactly what the PN-EN ISO 52016-1 monthly calculation of DiGi.Analytical.Building.Energy calculated (DiGi-Organization/DiGi.EnergyPlus#1). The comparison is meant to isolate the dynamics, the solar gains and the utilisation factor, so every steady-state input is DiGi's own.
        /// <para><b>Zone.</b> The building is one zone, as the monthly calculation treats it, with the volume and floor area of <paramref name="heatDemandResult"/>. Its surfaces are the faces of <see cref="BuildingModel.GetExternalShell(Side?, Orientation?, Orientation?, double)"/> with outward normals, counter-clockwise seen from outside; a face with holes is triangulated. Coordinates are moved so that the lowest corner of the model's bounding box is the origin, which keeps GIS coordinates out of the file and puts the ground at zero height.</para>
        /// <para><b>Boundaries.</b> Components are classified by <c>Regulation.Query.ConstructionRoles</c> and the floor rule of the monthly calculation. External walls, roofs and floors over outside air face outdoors. Party walls, components the calculation leaves out because their U is not known, and any other face of the shell are adiabatic. Floors on the ground or over the cellar face an other-side temperature held at the monthly mean external temperature of <paramref name="monthlyClimate"/>, with a construction whose U is H_g / ΣA (or H_U / ΣA over the cellar) of <paramref name="heatDemandResult"/>: the steady-state PN-EN ISO 13370 result of the monthly calculation, not a ground model of EnergyPlus. The thermal bridges H_tb are one hidden surface of U <see cref="Constants.Default.ThermalBridgeThermalTransmittance"/> whose outside face follows the hourly outdoor dry-bulb temperature.</para>
        /// <para><b>Constructions.</b> Layers are written outermost first, the order of a DiGi construction, and keep their mass. The steady-state U is DiGi's own (a declared U wins, as in the calculation): when the layers' resistance falls short of it, a <c>Material:NoMass</c> layer is added outermost; when it exceeds it, the resistance of every layer is scaled down. A construction with a declared U and no usable layers becomes one <c>Material:NoMass</c> of R = 1/U − R_si − R_se. Components bounding two spaces are internal mass: each face gets the half of the construction on its side.</para>
        /// <para><b>Openings.</b> Openings with a total solar energy transmittance are windows (glass doors for doors) of <c>WindowMaterial:SimpleGlazingSystem</c> with U the declared Uw over the outline and SHGC = g·A_g / A_w, so the glazed area is the one the calculation uses; EnergyPlus models the incidence angle itself, so the non-perpendicular incidence factor is not applied again. Openings without one are opaque doors of a <c>Material:NoMass</c> with the same U. An outline of more than four vertices is triangulated, since EnergyPlus accepts three or four.</para>
        /// <para><b>Loads.</b> The constant internal gain, the ventilation and infiltration flow (one flow carrying H_ve, at the standard air density) and the heating set-point are those of <paramref name="heatDemandResult"/>. An ideal-loads air system heats the zone with no limit and no cooling. The monthly sensible and total heating energy of the ideal loads and the envelope summary are reported.</para>
        /// <para>Every simplification that changes what is written - an excluded component, a triangulated face, a construction that could not be written as layers - is listed in comment lines at the top of the file.</para>
        /// </summary>
        /// <param name="buildingModel">The prepared building model: constructions, openings and internal conditions assigned. This value can be null.</param>
        /// <param name="ePWFile">The weather file, for the site location. This value can be null; EnergyPlus then takes the location from the weather file it runs with.</param>
        /// <param name="monthlyClimate">The monthly climate the heat demand was calculated with; its 12 external temperatures drive the ground and cellar boundaries. This value can be null.</param>
        /// <param name="typologyData">The typology data the heat demand was calculated with; its floor boundary and frame area fraction are used. This value can be null.</param>
        /// <param name="heatDemandResult">The heat demand calculated on <paramref name="buildingModel"/>; its heat transfer coefficients, gains, set-point, volume and floor area are written. This value can be null.</param>
        /// <param name="buildingModels_Neighbour">The neighbouring building models, written as building shading. This value can be null.</param>
        /// <param name="tolerance">The distance tolerance of the external shell and the triangulation.</param>
        /// <returns>The text of the IDF, or null when the model, the climate or the heat demand is null, the climate has not 12 temperatures, or the model has no external shell.</returns>
        public static string? ToEnergyPlus_String(this BuildingModel? buildingModel, EPWFile? ePWFile, MonthlyClimate? monthlyClimate, TypologyData? typologyData, HeatDemandResult? heatDemandResult, IEnumerable<BuildingModel?>? buildingModels_Neighbour, double tolerance = Core.Constants.Tolerance.Distance)
        {
            if (buildingModel is null || monthlyClimate is null || heatDemandResult is null)
            {
                return null;
            }

            List<double>? externalTemperatures = monthlyClimate.ExternalTemperatures;
            if (externalTemperatures is null || externalTemperatures.Count != 12 || externalTemperatures.Exists(double.IsNaN))
            {
                return null;
            }

            BoundingBox3D? boundingBox3D = buildingModel.GetBoundingBox();
            if (boundingBox3D is null)
            {
                return null;
            }

            Shell? shell = buildingModel.GetExternalShell(Side.External, Orientation.CounterClockwise, Orientation.Clockwise, tolerance);
            List<Face>? faces = shell?.PolygonalFaces;
            if (faces is null || faces.Count == 0)
            {
                return null;
            }

            Point3D origin = boundingBox3D.Min;
            double elevation_Min = origin.Z;

            List<string> notes = [];
            StringBuilder stringBuilder_Objects = new();
            Dictionary<string, string> materialNames = [];
            Dictionary<string, string> constructionNames = [];

            // EnergyPlus keeps materials, constructions and surfaces in separate name spaces.
            HashSet<string> names_Material = [];
            HashSet<string> names_Construction = [];
            HashSet<string> names_Surface = [];

            static string Format(double value)
            {
                string result = value.ToString("0.###############", CultureInfo.InvariantCulture);
                return result == "-0" ? "0" : result;
            }

            // Numbers in names and comments, short enough to read; values are written by Format.
            static string FormatShort(double value)
            {
                string result = value.ToString("0.###", CultureInfo.InvariantCulture);
                return result == "-0" ? "0" : result;
            }

            static string Sanitize(string? value, int maximumLength)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Unnamed";
                }

                StringBuilder stringBuilder = new();
                foreach (char @char in value!)
                {
                    stringBuilder.Append(@char == ',' || @char == ';' || @char == '!' || char.IsControl(@char) ? ' ' : @char);
                }

                string result = stringBuilder.ToString().Trim();
                return result.Length > maximumLength ? result.Substring(0, maximumLength).Trim() : result;
            }

            // A name that is unique in its name space, built from a readable stem.
            static string UniqueName(string stem, HashSet<string> names)
            {
                string result = Sanitize(stem, 90);
                int index = 2;
                string name = result;
                while (!names.Add(name))
                {
                    name = result + " " + index.ToString(CultureInfo.InvariantCulture);
                    index++;
                }

                return name;
            }

            void Add(string className, IList<string> fields, string? comment = null)
            {
                if (comment is not null)
                {
                    stringBuilder_Objects.Append("! ").AppendLine(Sanitize(comment, int.MaxValue));
                }

                stringBuilder_Objects.Append(className).AppendLine(",");
                for (int i = 0; i < fields.Count; i++)
                {
                    stringBuilder_Objects.Append("    ").Append(fields[i]).AppendLine(i == fields.Count - 1 ? ";" : ",");
                }

                stringBuilder_Objects.AppendLine();
            }

            List<string> Vertices(IList<Point3D> point3Ds)
            {
                List<string> result = [point3Ds.Count.ToString(CultureInfo.InvariantCulture)];
                foreach (Point3D point3D in point3Ds)
                {
                    result.Add(Format(point3D.X - origin.X) + ", " + Format(point3D.Y - origin.Y) + ", " + Format(point3D.Z - origin.Z));
                }

                return result;
            }

            static double Dot(Vector3D? vector3D_1, Vector3D? vector3D_2)
            {
                return vector3D_1 is null || vector3D_2 is null ? double.NaN : (vector3D_1.X * vector3D_2.X) + (vector3D_1.Y * vector3D_2.Y) + (vector3D_1.Z * vector3D_2.Z);
            }

            // The points of a ring turned to follow the given normal.
            static List<Point3D>? Oriented(IEnumerable<Point3D>? point3Ds, Vector3D? normal)
            {
                if (point3Ds is null)
                {
                    return null;
                }

                List<Point3D> result = [.. point3Ds];
                if (result.Count < 3)
                {
                    return null;
                }

                Vector3D? normal_Ring = Geometry.Spatial.Query.Normal(result);
                if (normal is not null && normal_Ring is not null && Dot(normal, normal_Ring) < 0)
                {
                    result.Reverse();
                }

                return result;
            }

            // The rings a face is written as: its outline, or its triangles when it has holes.
            List<List<Point3D>> Rings(IPolygonalFace3D? polygonalFace3D, Vector3D? normal, string name)
            {
                List<List<Point3D>> result = [];
                if (polygonalFace3D is not PolygonalFace3D polygonalFace3D_Temp)
                {
                    return result;
                }

                if (polygonalFace3D_Temp.InternalEdges is { Count: > 0 })
                {
                    List<Triangle3D>? triangle3Ds = polygonalFace3D_Temp.Triangulate(tolerance);
                    if (triangle3Ds is not null)
                    {
                        foreach (Triangle3D triangle3D in triangle3Ds)
                        {
                            if (Oriented(triangle3D.GetPoints(), normal) is List<Point3D> point3Ds_Triangle)
                            {
                                result.Add(point3Ds_Triangle);
                            }
                        }
                    }

                    notes.Add(name + ": face with holes written as " + result.Count.ToString(CultureInfo.InvariantCulture) + " triangles");
                    return result;
                }

                if (Oriented(polygonalFace3D_Temp.ExternalEdge?.GetPoints(), normal) is List<Point3D> point3Ds)
                {
                    result.Add(point3Ds);
                }

                return result;
            }

            static string LayerStem(IMaterial? material, double thickness)
            {
                return Sanitize((material as Analytical.Classes.Material)?.Name, 60) + " " + FormatShort(thickness * 1000.0) + " mm";
            }

            // The layers of a construction, outermost first, nothing written yet; null when one of them cannot be written.
            List<(string Stem, double Thickness, double Conductivity, double Density, double SpecificHeat, double ThermalResistance)>? Layers(List<IStructureLayer>? structureLayers, HeatFlowDirection heatFlowDirection)
            {
                if (structureLayers is null || structureLayers.Count == 0)
                {
                    return null;
                }

                List<(string Stem, double Thickness, double Conductivity, double Density, double SpecificHeat, double ThermalResistance)> result = [];
                foreach (IStructureLayer structureLayer in structureLayers)
                {
                    double thickness = structureLayer?.Thickness ?? double.NaN;
                    if (double.IsNaN(thickness) || thickness < 0)
                    {
                        return null;
                    }

                    if (thickness == 0)
                    {
                        continue;
                    }

                    IMaterial? material = buildingModel.GetMaterial(structureLayer);
                    double thermalResistance = material switch
                    {
                        OpaqueMaterial opaqueMaterial when opaqueMaterial.Conductivity > 0 => thickness / opaqueMaterial.Conductivity,
                        FluidMaterial => Analytical.Building.Query.AirLayerThermalResistance(thickness, heatFlowDirection),
                        _ => double.NaN,
                    };

                    if (double.IsNaN(thermalResistance) || thermalResistance <= 0)
                    {
                        return null;
                    }

                    if (material is OpaqueMaterial opaqueMaterial_Temp)
                    {
                        result.Add((LayerStem(material, thickness), thickness, opaqueMaterial_Temp.Conductivity, opaqueMaterial_Temp.Density, opaqueMaterial_Temp.SpecificHeat, thermalResistance));
                    }
                    else
                    {
                        result.Add((LayerStem(material, thickness), thickness, double.NaN, double.NaN, double.NaN, thermalResistance));
                    }
                }

                return result.Count == 0 ? null : result;
            }

            string MaterialName(string stem, double thickness, double conductivity, double density, double specificHeat)
            {
                string key = "Material|" + Format(thickness) + "|" + Format(conductivity) + "|" + Format(density) + "|" + Format(specificHeat) + "|" + stem;
                if (materialNames.TryGetValue(key, out string? name))
                {
                    return name;
                }

                name = UniqueName(stem, names_Material);
                materialNames[key] = name;
                Add("Material", [name, Constants.Default.Roughness, Format(thickness), Format(conductivity), Format(density), Format(specificHeat), Format(Constants.Default.ThermalAbsorptance), Format(Analytical.Building.Energy.Constants.Default.OpaqueSolarAbsorptance), Format(Constants.Default.VisibleAbsorptance)]);
                return name;
            }

            string NoMassName(string stem, double thermalResistance, double solarAbsorptance_Inside)
            {
                double thermalResistance_Temp = System.Math.Max(thermalResistance, Constants.Default.MinimumThermalResistance);
                string key = "NoMass|" + Format(thermalResistance_Temp) + "|" + Format(solarAbsorptance_Inside) + "|" + stem;
                if (materialNames.TryGetValue(key, out string? name))
                {
                    return name;
                }

                name = UniqueName(stem, names_Material);
                materialNames[key] = name;
                Add("Material:NoMass", [name, Constants.Default.Roughness, Format(thermalResistance_Temp), Format(Constants.Default.ThermalAbsorptance), Format(Analytical.Building.Energy.Constants.Default.OpaqueSolarAbsorptance), Format(Constants.Default.VisibleAbsorptance), Format(Constants.Default.ThermalAbsorptance), Format(solarAbsorptance_Inside), Format(Constants.Default.VisibleAbsorptance)]);
                return name;
            }

            // Writes a layer, its resistance scaled by the factor, and returns the name of its material. A layer keeps its mass when it has one.
            string LayerName((string Stem, double Thickness, double Conductivity, double Density, double SpecificHeat, double ThermalResistance) layer, double factor)
            {
                string stem = factor == 1.0 ? layer.Stem : layer.Stem + " x" + FormatShort(factor);
                bool mass = !double.IsNaN(layer.Conductivity) && !double.IsNaN(layer.Density) && layer.Density > 0 && !double.IsNaN(layer.SpecificHeat) && layer.SpecificHeat >= 100;
                return mass ? MaterialName(stem, layer.Thickness, layer.Conductivity / factor, layer.Density, layer.SpecificHeat) : NoMassName(stem, layer.ThermalResistance * factor, 0.7);
            }

            // A construction written once per distinct layer list.
            string ConstructionName(string stem, List<string> materials)
            {
                string key = string.Join("|", materials);
                if (constructionNames.TryGetValue(key, out string? name))
                {
                    return name;
                }

                name = UniqueName(stem, names_Construction);
                constructionNames[key] = name;
                List<string> fields = [name];
                fields.AddRange(materials);
                Add("Construction", fields);
                return name;
            }

            string NoMassConstructionName(string stem, double thermalResistance)
            {
                return ConstructionName(stem, [NoMassName(stem + " R " + FormatShort(thermalResistance), thermalResistance, 0.7)]);
            }

            // A construction of the given resistance between its faces: the layers, matched to it, or one massless layer.
            string? MatchedConstructionName(string stem, IPhysicalComponentConstruction? physicalComponentConstruction, HeatFlowDirection heatFlowDirection, double thermalResistance, string noteName)
            {
                if (double.IsNaN(thermalResistance))
                {
                    return null;
                }

                if (thermalResistance < Constants.Default.MinimumThermalResistance)
                {
                    notes.Add(noteName + ": resistance between faces " + FormatShort(thermalResistance) + " m2K/W raised to " + Format(Constants.Default.MinimumThermalResistance));
                    thermalResistance = Constants.Default.MinimumThermalResistance;
                }

                List<(string Stem, double Thickness, double Conductivity, double Density, double SpecificHeat, double ThermalResistance)>? layers = Layers(buildingModel.GetStructureLayers(physicalComponentConstruction), heatFlowDirection);
                if (layers is null)
                {
                    return NoMassConstructionName(stem, thermalResistance);
                }

                double thermalResistance_Layers = 0.0;
                foreach ((string _, double _, double _, double _, double _, double thermalResistance_Layer) in layers)
                {
                    thermalResistance_Layers += thermalResistance_Layer;
                }

                List<string> materials = [];
                if (System.Math.Abs(thermalResistance_Layers - thermalResistance) <= Core.Constants.Tolerance.Distance)
                {
                    materials = layers.ConvertAll(x => LayerName(x, 1.0));
                }
                else if (thermalResistance_Layers < thermalResistance)
                {
                    // Too little resistance: a massless layer outermost makes up the difference.
                    materials.Add(NoMassName(stem + " adjustment R " + FormatShort(thermalResistance - thermalResistance_Layers), thermalResistance - thermalResistance_Layers, 0.7));
                    materials.AddRange(layers.ConvertAll(x => LayerName(x, 1.0)));
                }
                else
                {
                    // Too much resistance: every layer keeps its mass and loses resistance in proportion.
                    double factor = thermalResistance / thermalResistance_Layers;
                    materials = layers.ConvertAll(x => LayerName(x, factor));
                    notes.Add(noteName + ": layer resistances scaled by " + FormatShort(factor) + " to the U of the calculation");
                }

                if (materials.Count > Constants.Default.MaximumLayerCount)
                {
                    notes.Add(noteName + ": " + materials.Count.ToString(CultureInfo.InvariantCulture) + " layers, written as one massless layer");
                    return NoMassConstructionName(stem, thermalResistance);
                }

                return ConstructionName(stem, materials);
            }

            // An adiabatic face keeps its layers as they are, for their mass.
            string AdiabaticConstructionName(string stem, IPhysicalComponentConstruction? physicalComponentConstruction, HeatFlowDirection heatFlowDirection, string noteName)
            {
                List<(string Stem, double Thickness, double Conductivity, double Density, double SpecificHeat, double ThermalResistance)>? layers = Layers(buildingModel.GetStructureLayers(physicalComponentConstruction), heatFlowDirection);
                if (layers is null || layers.Count > Constants.Default.MaximumLayerCount)
                {
                    notes.Add(noteName + ": adiabatic with a placeholder construction");
                    return NoMassConstructionName("Adiabatic placeholder", Constants.Default.PlaceholderThermalResistance);
                }

                return ConstructionName(stem, layers.ConvertAll(x => LayerName(x, 1.0)));
            }

            IPhysicalComponentConstruction? PhysicalComponentConstruction(IComponent component)
            {
                return component switch
                {
                    IWall wall => buildingModel.GetWallConstruction(wall),
                    IRoof roof => buildingModel.GetRoofConstruction(roof),
                    IFloor floor => buildingModel.GetFloorConstruction(floor),
                    _ => null,
                };
            }

            string Stem(IPhysicalComponentConstruction? physicalComponentConstruction, string role)
            {
                return Sanitize(physicalComponentConstruction?.Name ?? role, 60) + " (" + role + ")";
            }

            // Header.
            Add("Version", [Constants.Default.Version]);
            Add("SimulationControl", ["No", "No", "No", "No", "Yes", "No", "1"]);
            Add("Building", [buildingModel.Guid.ToString("D"), "0", "Suburbs", "0.04", "0.4", "FullExterior", Constants.Default.MaximumWarmupDays.ToString(CultureInfo.InvariantCulture), Constants.Default.MinimumWarmupDays.ToString(CultureInfo.InvariantCulture)], "One zone of a non-convex shell: beam solar falls on the floor (FullExterior).");
            Add("Timestep", [Constants.Default.TimestepsPerHour.ToString(CultureInfo.InvariantCulture)]);
            Add("GlobalGeometryRules", ["UpperLeftCorner", "Counterclockwise", "World"]);

            Location? location = ePWFile?.Location;
            if (location is not null)
            {
                Add("Site:Location", [Sanitize(location.City, 90), Format(location.Latitude), Format(location.Longitude), Format(location.TimeZone), Format(location.Elevation), "No"]);
            }

            Add("RunPeriod", ["Year", "1", "1", "", "12", "31", "", "Monday", "No", "No", "No", "Yes", "Yes"], "A typical year mixes the years of its months: the run period states none.");

            // Schedules.
            Add("ScheduleTypeLimits", ["Fraction", "0", "1", "Continuous"]);
            Add("ScheduleTypeLimits", ["Temperature", "", "", "Continuous", "Temperature"]);
            Add("ScheduleTypeLimits", ["Control Type", "0", "4", "Discrete"]);
            Add("Schedule:Constant", ["Always On", "Fraction", "1"]);
            Add("Schedule:Constant", ["Always Off", "Fraction", "0"]);
            Add("Schedule:Constant", ["Heating Setpoint Temperature", "Temperature", Format(heatDemandResult.InternalTemperature)]);
            Add("Schedule:Constant", ["Control Type", "Control Type", "1"]);

            int[] days = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
            List<string> fields_Ground = ["Monthly External Temperature", "Temperature"];
            for (int i = 0; i < 12; i++)
            {
                fields_Ground.Add("Through: " + (i + 1).ToString(CultureInfo.InvariantCulture) + "/" + days[i].ToString(CultureInfo.InvariantCulture));
                fields_Ground.Add("For: AllDays");
                fields_Ground.Add("Until: 24:00");
                fields_Ground.Add(Format(externalTemperatures[i]));
            }

            Add("Schedule:Compact", fields_Ground, "The monthly mean external temperatures of the monthly calculation.");

            // Zone.
            Add("Zone", [Constants.Name.Zone, "0", "0", "0", "0", "1", "1", "autocalculate", Format(heatDemandResult.HeatedVolume), Format(heatDemandResult.ReferenceFloorArea)]);

            // Classification, as the monthly calculation does it.
            FloorBoundary floorBoundary = typologyData?.FloorBoundary ?? FloorBoundary.Undefined;
            Dictionary<Guid, ConstructionRole> constructionRoles = buildingModel.ConstructionRoles(floorBoundary, tolerance) ?? [];

            Dictionary<Guid, IComponent> components = [];
            foreach (IComponent component in buildingModel.GetComponents<IComponent>() ?? [])
            {
                components[component.Guid] = component;
            }

            // Outside boundary of each face: Outdoors with a heat flow direction, Ground, or Adiabatic.
            List<(Face Face, IComponent Component, string Boundary, HeatFlowDirection HeatFlowDirection)> faces_Classified = [];
            double area_Ground = 0.0;
            foreach (Face face in faces)
            {
                if (face.UniqueReference is not GuidReference guidReference || !components.TryGetValue(guidReference.Guid, out IComponent? component))
                {
                    continue;
                }

                constructionRoles.TryGetValue(component.Guid, out ConstructionRole constructionRole);

                string boundary = "Adiabatic";
                HeatFlowDirection heatFlowDirection = HeatFlowDirection.Horizontal;
                switch (component)
                {
                    case IWall:
                        boundary = constructionRole == ConstructionRole.ExternalWall ? "Outdoors" : "Adiabatic";
                        break;

                    case IRoof:
                        boundary = constructionRole == ConstructionRole.Roof || constructionRole == ConstructionRole.FlatRoof ? "Outdoors" : "Adiabatic";
                        heatFlowDirection = HeatFlowDirection.Upward;
                        break;

                    case IFloor floor:
                        heatFlowDirection = HeatFlowDirection.Downward;
                        bool lowest = floor.GetBoundingBox() is BoundingBox3D boundingBox3D_Floor && boundingBox3D_Floor.Min.Z - elevation_Min <= Analytical.Building.Energy.Constants.Default.LevelTolerance;
                        if (constructionRole == ConstructionRole.GroundFloor || constructionRole == ConstructionRole.FloorOverUnheated || (constructionRole == ConstructionRole.IntermediateFloor && lowest))
                        {
                            boundary = "Ground";
                            area_Ground += face.GetArea();
                        }
                        else if (constructionRole == ConstructionRole.IntermediateFloor)
                        {
                            boundary = "Outdoors";
                        }

                        break;
                }

                if (boundary == "Adiabatic" && constructionRole != ConstructionRole.PartyWall)
                {
                    notes.Add(component.Guid.ToString("D") + ": " + constructionRole.ToString() + " face of the shell, adiabatic");
                }

                faces_Classified.Add((face, component, boundary, heatFlowDirection));
            }

            // The equivalent ground or cellar boundary.
            double heatTransferCoefficient_Ground = floorBoundary == FloorBoundary.Cellar ? heatDemandResult.UnheatedSpaceHeatTransferCoefficient : heatDemandResult.GroundHeatTransferCoefficient;
            double thermalTransmittance_Ground = area_Ground > 0 && heatTransferCoefficient_Ground > 0 ? heatTransferCoefficient_Ground / area_Ground : double.NaN;
            if (area_Ground > 0)
            {
                if (double.IsNaN(thermalTransmittance_Ground))
                {
                    notes.Add("ground floors: H " + FormatShort(heatTransferCoefficient_Ground) + " W/K, adiabatic");
                }
                else
                {
                    Add("SurfaceProperty:OtherSideCoefficients", [Constants.Name.GroundOtherSideCoefficients, "0", "0", "1", "0", "0", "0", "0", "Monthly External Temperature"], (floorBoundary == FloorBoundary.Cellar ? "Cellar: H_U " : "Ground: H_g ") + FormatShort(heatTransferCoefficient_Ground) + " W/K over " + FormatShort(area_Ground) + " m2, U " + FormatShort(thermalTransmittance_Ground) + " W/(m2K) to the monthly mean external temperature.");
                }
            }

            // Shell surfaces and their openings.
            int count_Opening = 0;
            foreach ((Face face, IComponent component, string boundary, HeatFlowDirection heatFlowDirection) in faces_Classified)
            {
                string name = component.Guid.ToString("D");
                IPhysicalComponentConstruction? physicalComponentConstruction = PhysicalComponentConstruction(component);
                Vector3D? normal = face.Plane?.Normal;

                string surfaceType = component switch
                {
                    IRoof => "Roof",
                    IFloor => "Floor",
                    _ => "Wall",
                };

                string boundary_Temp = boundary;
                string? constructionName = null;
                switch (boundary)
                {
                    case "Outdoors":
                        double thermalTransmittance = buildingModel.ThermalTransmittance(physicalComponentConstruction, heatFlowDirection);
                        if (double.IsNaN(thermalTransmittance) || thermalTransmittance <= 0)
                        {
                            notes.Add(name + ": U not known, excluded by the calculation, adiabatic");
                            boundary_Temp = "Adiabatic";
                            break;
                        }

                        double thermalResistance = (1.0 / thermalTransmittance) - Analytical.Building.Query.SurfaceResistance(heatFlowDirection, true) - Analytical.Building.Query.SurfaceResistance(heatFlowDirection, false);
                        constructionName = MatchedConstructionName(Stem(physicalComponentConstruction, surfaceType + " U " + FormatShort(thermalTransmittance)), physicalComponentConstruction, heatFlowDirection, thermalResistance, name);
                        break;

                    case "Ground":
                        if (double.IsNaN(thermalTransmittance_Ground))
                        {
                            boundary_Temp = "Adiabatic";
                            break;
                        }

                        constructionName = MatchedConstructionName(Stem(physicalComponentConstruction, "Ground floor U " + FormatShort(thermalTransmittance_Ground)), physicalComponentConstruction, HeatFlowDirection.Downward, (1.0 / thermalTransmittance_Ground) - Analytical.Building.Query.SurfaceResistance(HeatFlowDirection.Downward, true), name);
                        break;
                }

                constructionName ??= AdiabaticConstructionName(Stem(physicalComponentConstruction, "Adiabatic " + surfaceType), physicalComponentConstruction, heatFlowDirection, name);

                List<List<Point3D>> rings = Rings(face, normal, name);
                List<string> surfaceNames = [];
                for (int i = 0; i < rings.Count; i++)
                {
                    string surfaceName = rings.Count == 1 ? name : name + " " + (i + 1).ToString(CultureInfo.InvariantCulture);
                    surfaceNames.Add(surfaceName);

                    List<string> fields = [surfaceName, surfaceType, constructionName, Constants.Name.Zone, ""];
                    switch (boundary_Temp)
                    {
                        case "Outdoors":
                            fields.AddRange(["Outdoors", "", "SunExposed", "WindExposed", "autocalculate"]);
                            break;

                        case "Ground":
                            fields.AddRange(["OtherSideCoefficients", Constants.Name.GroundOtherSideCoefficients, "NoSun", "NoWind", "autocalculate"]);
                            break;

                        default:
                            fields.AddRange(["Adiabatic", "", "NoSun", "NoWind", "autocalculate"]);
                            break;
                    }

                    fields.AddRange(Vertices(rings[i]));
                    Add("BuildingSurface:Detailed", fields);
                }

                List<IOpening>? openings = buildingModel.GetOpenings<IOpening>(component);
                if (openings is null || openings.Count == 0)
                {
                    continue;
                }

                if (boundary_Temp != "Outdoors")
                {
                    notes.Add(name + ": " + openings.Count.ToString(CultureInfo.InvariantCulture) + " openings on a surface that is not outdoors, left out");
                    continue;
                }

                foreach (IOpening opening in openings)
                {
                    string name_Opening = opening.Guid.ToString("D");
                    IOpeningConstruction? openingConstruction = buildingModel.GetOpeningConstruction<IOpeningConstruction>(opening);
                    if (Analytical.Building.Query.Surface3D(opening, openingConstruction) is not PolygonalFace3D polygonalFace3D_Opening)
                    {
                        notes.Add(name_Opening + ": outline cannot be placed, left out");
                        continue;
                    }

                    double area = polygonalFace3D_Opening.GetArea();
                    double thermalTransmittance = double.NaN;
                    if (openingConstruction is not null && openingConstruction.TryGetValue(new EnumParameterDefinition(OpeningConstructionParameter.ThermalTransmittance), out double thermalTransmittance_Declared))
                    {
                        thermalTransmittance = thermalTransmittance_Declared;
                    }

                    if (double.IsNaN(thermalTransmittance))
                    {
                        thermalTransmittance = buildingModel.ThermalTransmittance(openingConstruction);
                    }

                    if (double.IsNaN(thermalTransmittance) || thermalTransmittance <= 0 || double.IsNaN(area) || area <= 0)
                    {
                        notes.Add(name_Opening + ": U or area not known, excluded by the calculation, left out");
                        continue;
                    }

                    double totalSolarEnergyTransmittance = double.NaN;
                    if (openingConstruction is not null && openingConstruction.TryGetValue(new EnumParameterDefinition(OpeningConstructionParameter.TotalSolarEnergyTransmittance), out double totalSolarEnergyTransmittance_Declared))
                    {
                        totalSolarEnergyTransmittance = totalSolarEnergyTransmittance_Declared;
                    }

                    string fenestrationType;
                    string constructionName_Opening;
                    if (!double.IsNaN(totalSolarEnergyTransmittance) && totalSolarEnergyTransmittance > 0)
                    {
                        double area_Glazed = 0.0;
                        foreach (ISurface3D surface3D_Pane in Analytical.Building.Query.Surface3Ds(opening, openingConstruction, OpeningConstructionPart.Pane) ?? [])
                        {
                            if (surface3D_Pane is PolygonalFace3D polygonalFace3D_Pane && !double.IsNaN(polygonalFace3D_Pane.GetArea()))
                            {
                                area_Glazed += polygonalFace3D_Pane.GetArea();
                            }
                        }

                        if (area_Glazed <= 0)
                        {
                            double frameAreaFraction = typologyData?.FrameAreaFraction ?? double.NaN;
                            if (double.IsNaN(frameAreaFraction) || frameAreaFraction < 0 || frameAreaFraction >= 1)
                            {
                                frameAreaFraction = Analytical.Building.Energy.Constants.Default.FrameAreaFraction;
                            }

                            area_Glazed = (1.0 - frameAreaFraction) * area;
                        }

                        double solarHeatGainCoefficient = System.Math.Min(System.Math.Max(totalSolarEnergyTransmittance * area_Glazed / area, 0.001), 0.999);
                        string key = "Glazing|" + Format(thermalTransmittance) + "|" + Format(solarHeatGainCoefficient);
                        if (!materialNames.TryGetValue(key, out string? name_Glazing))
                        {
                            name_Glazing = UniqueName("Glazing U " + FormatShort(thermalTransmittance) + " SHGC " + FormatShort(solarHeatGainCoefficient), names_Material);
                            materialNames[key] = name_Glazing;
                            Add("WindowMaterial:SimpleGlazingSystem", [name_Glazing, Format(thermalTransmittance), Format(solarHeatGainCoefficient)]);
                        }

                        constructionName_Opening = ConstructionName(name_Glazing, [name_Glazing]);
                        fenestrationType = opening is IDoor ? "GlassDoor" : "Window";
                    }
                    else
                    {
                        double thermalResistance = (1.0 / thermalTransmittance) - Analytical.Building.Query.SurfaceResistance(heatFlowDirection, true) - Analytical.Building.Query.SurfaceResistance(heatFlowDirection, false);
                        constructionName_Opening = NoMassConstructionName("Door U " + FormatShort(thermalTransmittance), thermalResistance);
                        fenestrationType = "Door";
                    }

                    // EnergyPlus takes three or four vertices: a larger outline is triangulated.
                    List<List<Point3D>> rings_Opening = [];
                    List<Point3D>? point3Ds_Opening = Oriented(polygonalFace3D_Opening.ExternalEdge?.GetPoints(), normal);
                    if (point3Ds_Opening is not null && point3Ds_Opening.Count <= 4)
                    {
                        rings_Opening.Add(point3Ds_Opening);
                    }
                    else
                    {
                        foreach (Triangle3D triangle3D in polygonalFace3D_Opening.Triangulate(tolerance) ?? [])
                        {
                            if (Oriented(triangle3D.GetPoints(), normal) is List<Point3D> point3Ds_Triangle)
                            {
                                rings_Opening.Add(point3Ds_Triangle);
                            }
                        }

                        notes.Add(name_Opening + ": outline of " + (point3Ds_Opening?.Count ?? 0).ToString(CultureInfo.InvariantCulture) + " vertices written as " + rings_Opening.Count.ToString(CultureInfo.InvariantCulture) + " triangles");
                    }

                    // The host is the surface, or the triangle of a face with holes, that holds the centre of the outline.
                    string surfaceName_Host = surfaceNames.Count > 0 ? surfaceNames[0] : name;
                    if (surfaceNames.Count > 1 && polygonalFace3D_Opening.ExternalEdge?.GetCentroid() is Point3D centroid)
                    {
                        for (int i = 0; i < rings.Count; i++)
                        {
                            if (rings[i].Count == 3 && new Triangle3D(rings[i][0], rings[i][1], rings[i][2]).InRange(centroid, Core.Constants.Tolerance.MacroDistance))
                            {
                                surfaceName_Host = surfaceNames[i];
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < rings_Opening.Count; i++)
                    {
                        List<string> fields = [rings_Opening.Count == 1 ? name_Opening : name_Opening + " " + (i + 1).ToString(CultureInfo.InvariantCulture), fenestrationType, constructionName_Opening, surfaceName_Host, "", "autocalculate", "", "1"];
                        fields.AddRange(Vertices(rings_Opening[i]));
                        Add("FenestrationSurface:Detailed", fields);
                        count_Opening++;
                    }
                }
            }

            // Thermal bridges: one hidden wall west of the building, below ground, whose outside face follows the outdoor dry-bulb temperature.
            double thermalBridgeHeatTransferCoefficient = heatDemandResult.ThermalBridgeHeatTransferCoefficient;
            if (!double.IsNaN(thermalBridgeHeatTransferCoefficient) && thermalBridgeHeatTransferCoefficient > 0)
            {
                double area = thermalBridgeHeatTransferCoefficient / Constants.Default.ThermalBridgeThermalTransmittance;
                double side = System.Math.Sqrt(area);
                double x = origin.X - Constants.Default.ThermalBridgeOffset;

                Add("SurfaceProperty:OtherSideCoefficients", [Constants.Name.ThermalBridgeOtherSideCoefficients, "0", "0", "0", "1"], "Thermal bridges: H_tb " + FormatShort(thermalBridgeHeatTransferCoefficient) + " W/K as " + FormatShort(area) + " m2 of U " + FormatShort(Constants.Default.ThermalBridgeThermalTransmittance) + " W/(m2K) to the outdoor dry-bulb temperature.");

                double thermalResistance_ThermalBridge = (1.0 / Constants.Default.ThermalBridgeThermalTransmittance) - Analytical.Building.Query.SurfaceResistance(HeatFlowDirection.Horizontal, true);
                string constructionName = ConstructionName("Thermal bridges", [NoMassName("Thermal bridges R " + FormatShort(thermalResistance_ThermalBridge), thermalResistance_ThermalBridge, 0.0)]);

                List<string> fields = [Constants.Name.ThermalBridgeSurface, "Wall", constructionName, Constants.Name.Zone, "", "OtherSideCoefficients", Constants.Name.ThermalBridgeOtherSideCoefficients, "NoSun", "NoWind", "autocalculate"];
                fields.AddRange(Vertices([new Point3D(x, origin.Y + side, origin.Z), new Point3D(x, origin.Y + side, origin.Z - side), new Point3D(x, origin.Y, origin.Z - side), new Point3D(x, origin.Y, origin.Z)]));
                Add("BuildingSurface:Detailed", fields);
            }

            // Internal mass: each face of a component between two spaces gets the half of the construction on its side.
            int count_InternalMass = 0;
            foreach (IComponent component in components.Values)
            {
                if ((buildingModel.GetSpaces(component)?.Count ?? 0) != 2)
                {
                    continue;
                }

                string name = component.Guid.ToString("D");
                double area = Analytical.Building.Energy.Query.Area(component);
                IPhysicalComponentConstruction? physicalComponentConstruction = PhysicalComponentConstruction(component);
                HeatFlowDirection heatFlowDirection = component is IWall ? HeatFlowDirection.Horizontal : HeatFlowDirection.Downward;
                List<IStructureLayer>? structureLayers = buildingModel.GetStructureLayers(physicalComponentConstruction);
                if (double.IsNaN(area) || area <= 0 || structureLayers is null || Layers(structureLayers, heatFlowDirection) is null)
                {
                    notes.Add(name + ": internal component without area or layers, left out of the internal mass");
                    continue;
                }

                List<(IStructureLayer StructureLayer, IMaterial? Material)> layers = structureLayers.ConvertAll(x => (x, buildingModel.GetMaterial(x)));
                double thickness_Total = 0.0;
                foreach ((IStructureLayer structureLayer, IMaterial? _) in layers)
                {
                    thickness_Total += structureLayer.Thickness;
                }

                // Side 0 faces the first layer, side 1 the last; EnergyPlus exposes the last layer of an internal mass to the zone.
                for (int side = 0; side < 2; side++)
                {
                    List<string> materials = [];
                    double thickness_Remaining = thickness_Total / 2.0;
                    for (int i = 0; i < layers.Count && thickness_Remaining > Core.Constants.Tolerance.Distance; i++)
                    {
                        (IStructureLayer structureLayer, IMaterial? material) = layers[side == 0 ? i : layers.Count - 1 - i];
                        double thickness = System.Math.Min(structureLayer.Thickness, thickness_Remaining);
                        thickness_Remaining -= structureLayer.Thickness;
                        if (thickness <= 0)
                        {
                            continue;
                        }

                        string stem = LayerStem(material, thickness);
                        materials.Add(material switch
                        {
                            OpaqueMaterial opaqueMaterial when opaqueMaterial.Density > 0 && opaqueMaterial.SpecificHeat >= 100 => MaterialName(stem, thickness, opaqueMaterial.Conductivity, opaqueMaterial.Density, opaqueMaterial.SpecificHeat),
                            OpaqueMaterial opaqueMaterial => NoMassName(stem, thickness / opaqueMaterial.Conductivity, 0.7),
                            _ => NoMassName(stem, Analytical.Building.Query.AirLayerThermalResistance(structureLayer.Thickness, heatFlowDirection) * thickness / structureLayer.Thickness, 0.7),
                        });
                    }

                    // Built from the middle outwards, so the exposed face is last.
                    materials.Reverse();
                    if (materials.Count == 0 || materials.Count > Constants.Default.MaximumLayerCount)
                    {
                        continue;
                    }

                    string constructionName = ConstructionName(Stem(physicalComponentConstruction, "Internal half"), materials);
                    Add("InternalMass", [name + " " + (side + 1).ToString(CultureInfo.InvariantCulture), constructionName, Constants.Name.Zone, "", Format(area)]);
                    count_InternalMass++;
                }
            }

            // Neighbours as building shading.
            int count_Shading = 0;
            if (buildingModels_Neighbour is not null)
            {
                foreach (BuildingModel? buildingModel_Neighbour in buildingModels_Neighbour)
                {
                    if (buildingModel_Neighbour is null || buildingModel_Neighbour.Guid == buildingModel.Guid)
                    {
                        continue;
                    }

                    List<IPolygonalFace3D> polygonalFace3Ds = [];
                    if (buildingModel_Neighbour.GetExternalShell(tolerance: tolerance)?.PolygonalFaces is List<Face> faces_Neighbour)
                    {
                        polygonalFace3Ds.AddRange(faces_Neighbour);
                    }
                    else
                    {
                        foreach (IComponent component in buildingModel_Neighbour.GetComponents<IComponent>() ?? [])
                        {
                            if (Analytical.Building.Query.Surface3D(component) is IPolygonalFace3D polygonalFace3D)
                            {
                                polygonalFace3Ds.Add(polygonalFace3D);
                            }
                        }
                    }

                    string name = buildingModel_Neighbour.Guid.ToString("D");
                    for (int i = 0; i < polygonalFace3Ds.Count; i++)
                    {
                        List<List<Point3D>> rings = Rings(polygonalFace3Ds[i], null, name + " face " + (i + 1).ToString(CultureInfo.InvariantCulture));
                        for (int j = 0; j < rings.Count; j++)
                        {
                            List<string> fields = [UniqueName(name + " " + (i + 1).ToString(CultureInfo.InvariantCulture) + (rings.Count == 1 ? string.Empty : "." + (j + 1).ToString(CultureInfo.InvariantCulture)), names_Surface), ""];
                            fields.AddRange(Vertices(rings[j]));
                            Add("Shading:Building:Detailed", fields);
                            count_Shading++;
                        }
                    }
                }
            }

            // Loads and the ideal-loads air system.
            double internalGain = 0.0;
            double duration = 0.0;
            List<double> internalHeatGains = heatDemandResult.InternalHeatGains ?? [];
            for (int i = 0; i < internalHeatGains.Count && i < 12; i++)
            {
                internalGain += internalHeatGains[i] * 1000.0;
                duration += Analytical.Building.Energy.Query.MonthDuration(i + 1);
            }

            if (duration > 0 && internalGain > 0)
            {
                Add("OtherEquipment", ["Internal Gains", "None", Constants.Name.Zone, "Always On", "EquipmentLevel", Format(internalGain / duration), "", "", "0", "0", "0"], "The constant internal gain of the monthly calculation [W].");
            }

            double ventilationHeatTransferCoefficient = heatDemandResult.VentilationHeatTransferCoefficient;
            if (!double.IsNaN(ventilationHeatTransferCoefficient) && ventilationHeatTransferCoefficient > 0)
            {
                Add("ZoneVentilation:DesignFlowRate", ["Ventilation and Infiltration", Constants.Name.Zone, "Always On", "Flow/Zone", Format(ventilationHeatTransferCoefficient / Analytical.Building.Energy.Constants.Default.AirHeatCapacity), "", "", "", "Natural", "0", "1", "1", "0", "0", "0", "-100", "", "100", "", "-100", "", "-100", "", "100", "", "40", "Standard"], "H_ve " + FormatShort(ventilationHeatTransferCoefficient) + " W/K of the monthly calculation as one constant flow [m3/s].");
            }

            Add("ZoneHVAC:IdealLoadsAirSystem", [Constants.Name.IdealLoadsAirSystem, "", "Zone Inlet Node", "", "", "50", "13", "0.0156", "0.0077", "NoLimit", "", "", "NoLimit", "", "", "", "Always Off"]);
            Add("ZoneHVAC:EquipmentList", ["Equipment", "SequentialLoad", "ZoneHVAC:IdealLoadsAirSystem", Constants.Name.IdealLoadsAirSystem, "1", "1", "", ""]);
            Add("ZoneHVAC:EquipmentConnections", [Constants.Name.Zone, "Equipment", "Zone Inlet Node", "", "Zone Air Node", "Zone Return Node"]);
            Add("ThermostatSetpoint:SingleHeating", ["Heating Setpoint", "Heating Setpoint Temperature"]);
            Add("ZoneControl:Thermostat", ["Thermostat", Constants.Name.Zone, "Control Type", "ThermostatSetpoint:SingleHeating", "Heating Setpoint"]);

            // Output.
            Add("Output:Variable", ["*", "Zone Ideal Loads Zone Sensible Heating Energy", "Monthly"]);
            Add("Output:Variable", ["*", "Zone Ideal Loads Zone Total Heating Energy", "Monthly"]);
            Add("Output:Table:SummaryReports", ["EnvelopeSummary"]);
            Add("OutputControl:Table:Style", ["Comma", "None"]);
            Add("Output:VariableDictionary", ["IDF"]);

            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("! Written by DiGi.EnergyPlus for EnergyPlus " + Constants.Default.Version + " (DiGi-Organization/DiGi.EnergyPlus#1).");
            stringBuilder.AppendLine("! Building model " + buildingModel.Guid.ToString("D") + "; origin moved to " + Format(origin.X) + ", " + Format(origin.Y) + ", " + Format(origin.Z) + ".");
            stringBuilder.AppendLine("! Openings " + count_Opening.ToString(CultureInfo.InvariantCulture) + ", internal mass surfaces " + count_InternalMass.ToString(CultureInfo.InvariantCulture) + ", shading surfaces " + count_Shading.ToString(CultureInfo.InvariantCulture) + ".");
            foreach (string note in notes)
            {
                stringBuilder.Append("! Note: ").AppendLine(Sanitize(note, int.MaxValue));
            }

            stringBuilder.AppendLine();
            stringBuilder.Append(stringBuilder_Objects);

            return stringBuilder.ToString();
        }
    }
}
