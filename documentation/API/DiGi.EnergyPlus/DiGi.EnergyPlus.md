#### [DiGi\.EnergyPlus](DiGi.EnergyPlus.Overview.md 'DiGi\.EnergyPlus\.Overview')

## DiGi\.EnergyPlus Namespace
### Classes

<a name='DiGi.EnergyPlus.Convert'></a>

## Convert Class

```csharp
public static class Convert
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Convert
### Methods

<a name='DiGi.EnergyPlus.Convert.ToDiGi_EnergyPlusResult(string)'></a>

## Convert\.ToDiGi\_EnergyPlusResult\(string\) Method

Reads the result of an EnergyPlus run from its run directory \(DiGi\-Organization/DiGi\.EnergyPlus\#1\), as `Modify.RunAsync` leaves it\.

From [Error](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.Error 'DiGi\.EnergyPlus\.Constants\.FileName\.Error'): the version, and the warning and severe error counts of the completed run. From [Variables](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.Variables 'DiGi\.EnergyPlus\.Constants\.FileName\.Variables'): the monthly [HeatingEnergyVariable](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.Name.HeatingEnergyVariable 'DiGi\.EnergyPlus\.Constants\.Name\.HeatingEnergyVariable') of every ideal-loads air system, converted from J to kWh, January to December. From [Tables](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.Tables 'DiGi\.EnergyPlus\.Constants\.FileName\.Tables'), when present: the U-factor with film of every opaque external surface and the glass U-factor of every window of the envelope summary.

```csharp
public static DiGi.EnergyPlus.Classes.EnergyPlusResult? ToDiGi_EnergyPlusResult(string? directory);
```
#### Parameters

<a name='DiGi.EnergyPlus.Convert.ToDiGi_EnergyPlusResult(string).directory'></a>

`directory` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The run directory\. This value can be null\.

#### Returns
[EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult')  
The [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult'), or null when the directory or a required file is missing, the run did not complete successfully, or no system reports 12 monthly heating energies\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double)'></a>

## Convert\.ToEnergyPlus\_String\(this BuildingModel, EPWFile, MonthlyClimate, TypologyData, HeatDemandResult, IEnumerable\<BuildingModel\>, double\) Method

Writes a prepared building model as an EnergyPlus input file \(IDF\) for [Version](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.Default.Version 'DiGi\.EnergyPlus\.Constants\.Default\.Version'), so that EnergyPlus calculates the heating need of exactly what the PN\-EN ISO 52016\-1 monthly calculation of DiGi\.Analytical\.Building\.Energy calculated \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\. The comparison is meant to isolate the dynamics, the solar gains and the utilisation factor, so every steady\-state input is DiGi's own\.

<b>Zone.</b> The building is one zone, as the monthly calculation treats it, with the volume and floor area of [heatDemandResult](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).heatDemandResult 'DiGi\.EnergyPlus\.Convert\.ToEnergyPlus\_String\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, DiGi\.EPW\.Classes\.EPWFile, DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate, DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData, DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, double\)\.heatDemandResult'). Its surfaces are the faces of [DiGi\.Analytical\.Building\.Classes\.BuildingModel\.GetExternalShell\(System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Side\},System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Orientation\},System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Orientation\},System\.Double\)](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel.getexternalshell#digi-analytical-building-classes-buildingmodel-getexternalshell(system-nullable{digi-geometry-core-enums-side}-system-nullable{digi-geometry-core-enums-orientation}-system-nullable{digi-geometry-core-enums-orientation}-system-double) 'DiGi\.Analytical\.Building\.Classes\.BuildingModel\.GetExternalShell\(System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Side\},System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Orientation\},System\.Nullable\{DiGi\.Geometry\.Core\.Enums\.Orientation\},System\.Double\)') with outward normals, counter-clockwise seen from outside; a face with holes is triangulated. Coordinates are moved so that the lowest corner of the model's bounding box is the origin, which keeps GIS coordinates out of the file and puts the ground at zero height.

<b>Boundaries.</b> Components are classified by `Regulation.Query.ConstructionRoles` and the floor rule of the monthly calculation. External walls, roofs and floors over outside air face outdoors. Party walls, components the calculation leaves out because their U is not known, and any other face of the shell are adiabatic. Floors on the ground or over the cellar face an other-side temperature held at the monthly mean external temperature of [monthlyClimate](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).monthlyClimate 'DiGi\.EnergyPlus\.Convert\.ToEnergyPlus\_String\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, DiGi\.EPW\.Classes\.EPWFile, DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate, DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData, DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, double\)\.monthlyClimate'), with a construction whose U is H_g / ΣA (or H_U / ΣA over the cellar) of [heatDemandResult](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).heatDemandResult 'DiGi\.EnergyPlus\.Convert\.ToEnergyPlus\_String\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, DiGi\.EPW\.Classes\.EPWFile, DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate, DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData, DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, double\)\.heatDemandResult'): the steady-state PN-EN ISO 13370 result of the monthly calculation, not a ground model of EnergyPlus. The thermal bridges H_tb are one hidden surface of U [ThermalBridgeThermalTransmittance](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.Default.ThermalBridgeThermalTransmittance 'DiGi\.EnergyPlus\.Constants\.Default\.ThermalBridgeThermalTransmittance') whose outside face follows the hourly outdoor dry-bulb temperature.

<b>Constructions.</b> Layers are written outermost first, the order of a DiGi construction, and keep their mass. The steady-state U is DiGi's own (a declared U wins, as in the calculation): when the layers' resistance falls short of it, a `Material:NoMass` layer is added outermost; when it exceeds it, the resistance of every layer is scaled down. A construction with a declared U and no usable layers becomes one `Material:NoMass` of R = 1/U − R_si − R_se. Components bounding two spaces are internal mass: each face gets the half of the construction on its side.

<b>Openings.</b> Openings with a total solar energy transmittance are windows (glass doors for doors) of `WindowMaterial:SimpleGlazingSystem` with U the declared Uw over the outline and SHGC = g·A_g / A_w, so the glazed area is the one the calculation uses; EnergyPlus models the incidence angle itself, so the non-perpendicular incidence factor is not applied again. Openings without one are opaque doors of a `Material:NoMass` with the same U. An outline of more than four vertices is triangulated, since EnergyPlus accepts three or four.

<b>Loads.</b> The constant internal gain, the ventilation and infiltration flow (one flow carrying H_ve, at the standard air density) and the heating set-point are those of [heatDemandResult](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).heatDemandResult 'DiGi\.EnergyPlus\.Convert\.ToEnergyPlus\_String\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, DiGi\.EPW\.Classes\.EPWFile, DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate, DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData, DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, double\)\.heatDemandResult'). An ideal-loads air system heats the zone with no limit and no cooling. The monthly sensible and total heating energy of the ideal loads and the envelope summary are reported.

Every simplification that changes what is written - an excluded component, a triangulated face, a construction that could not be written as layers - is listed in comment lines at the top of the file.

```csharp
public static string? ToEnergyPlus_String(this DiGi.Analytical.Building.Classes.BuildingModel? buildingModel, DiGi.EPW.Classes.EPWFile? ePWFile, DiGi.Analytical.Building.Energy.Classes.MonthlyClimate? monthlyClimate, DiGi.Analytical.Building.Regulation.Classes.TypologyData? typologyData, DiGi.Analytical.Building.Energy.Classes.HeatDemandResult? heatDemandResult, System.Collections.Generic.IEnumerable<DiGi.Analytical.Building.Classes.BuildingModel?>? buildingModels_Neighbour, double tolerance=1E-06);
```
#### Parameters

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).buildingModel'></a>

`buildingModel` [DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')

The prepared building model: constructions, openings and internal conditions assigned\. This value can be null\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).ePWFile'></a>

`ePWFile` [DiGi\.EPW\.Classes\.EPWFile](https://learn.microsoft.com/en-us/dotnet/api/digi.epw.classes.epwfile 'DiGi\.EPW\.Classes\.EPWFile')

The weather file, for the site location\. This value can be null; EnergyPlus then takes the location from the weather file it runs with\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).monthlyClimate'></a>

`monthlyClimate` [DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.energy.classes.monthlyclimate 'DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate')

The monthly climate the heat demand was calculated with; its 12 external temperatures drive the ground and cellar boundaries\. This value can be null\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).typologyData'></a>

`typologyData` [DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.regulation.classes.typologydata 'DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData')

The typology data the heat demand was calculated with; its floor boundary and frame area fraction are used\. This value can be null\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).heatDemandResult'></a>

`heatDemandResult` [DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.energy.classes.heatdemandresult 'DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult')

The heat demand calculated on [buildingModel](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).buildingModel 'DiGi\.EnergyPlus\.Convert\.ToEnergyPlus\_String\(this DiGi\.Analytical\.Building\.Classes\.BuildingModel, DiGi\.EPW\.Classes\.EPWFile, DiGi\.Analytical\.Building\.Energy\.Classes\.MonthlyClimate, DiGi\.Analytical\.Building\.Regulation\.Classes\.TypologyData, DiGi\.Analytical\.Building\.Energy\.Classes\.HeatDemandResult, System\.Collections\.Generic\.IEnumerable\<DiGi\.Analytical\.Building\.Classes\.BuildingModel\>, double\)\.buildingModel'); its heat transfer coefficients, gains, set\-point, volume and floor area are written\. This value can be null\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).buildingModels_Neighbour'></a>

`buildingModels_Neighbour` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.Analytical\.Building\.Classes\.BuildingModel](https://learn.microsoft.com/en-us/dotnet/api/digi.analytical.building.classes.buildingmodel 'DiGi\.Analytical\.Building\.Classes\.BuildingModel')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The neighbouring building models, written as building shading\. This value can be null\.

<a name='DiGi.EnergyPlus.Convert.ToEnergyPlus_String(thisDiGi.Analytical.Building.Classes.BuildingModel,DiGi.EPW.Classes.EPWFile,DiGi.Analytical.Building.Energy.Classes.MonthlyClimate,DiGi.Analytical.Building.Regulation.Classes.TypologyData,DiGi.Analytical.Building.Energy.Classes.HeatDemandResult,System.Collections.Generic.IEnumerable_DiGi.Analytical.Building.Classes.BuildingModel_,double).tolerance'></a>

`tolerance` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The distance tolerance of the external shell and the triangulation\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The text of the IDF, or null when the model, the climate or the heat demand is null, the climate has not 12 temperatures, or the model has no external shell\.

<a name='DiGi.EnergyPlus.Modify'></a>

## Modify Class

```csharp
public static class Modify
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Modify
### Methods

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken)'></a>

## Modify\.RunAsync\(string, string, string, string, CancellationToken\) Method

Runs EnergyPlus on an input file in a run directory \(DiGi\-Organization/DiGi\.EnergyPlus\#1\): writes the IDF as [Input](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.Input 'DiGi\.EnergyPlus\.Constants\.FileName\.Input'), copies the weather file as [Weather](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.Weather 'DiGi\.EnergyPlus\.Constants\.FileName\.Weather'), and runs the command line with ReadVarsESO \(`-r`\), so that `Convert.ToDiGi_EnergyPlusResult` can read the directory afterwards\.

The output files the reader takes are deleted first, so a failed run never leaves the previous run's results to be read. The run is ended when it has written nothing for [RunInactivityTimeout](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.Default.RunInactivityTimeout 'DiGi\.EnergyPlus\.Constants\.Default\.RunInactivityTimeout') minutes or when [cancellationToken](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).cancellationToken 'DiGi\.EnergyPlus\.Modify\.RunAsync\(string, string, string, string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled. A failure is logged through DiGi.Serilog with the exit code and the trailing error output.

```csharp
public static System.Threading.Tasks.Task<bool> RunAsync(string? text, string? path_EPW, string? path_Executable, string? directory, System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text of the IDF, as `Convert.ToEnergyPlus_String` writes it\. This value can be null\.

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).path_EPW'></a>

`path_EPW` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path of the weather file\. This value can be null\.

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).path_Executable'></a>

`path_Executable` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path of the EnergyPlus executable, from [ExecutablePath\(string\)](DiGi.EnergyPlus.md#DiGi.EnergyPlus.Query.ExecutablePath(string) 'DiGi\.EnergyPlus\.Query\.ExecutablePath\(string\)')\. This value can be null\.

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).directory'></a>

`directory` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The run directory; created when missing\. This value can be null\.

<a name='DiGi.EnergyPlus.Modify.RunAsync(string,string,string,string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

The token that cancels the run\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
True when EnergyPlus exited with 0 and reports that it completed successfully; false when an argument is missing, a file does not exist, or the run failed\.

<a name='DiGi.EnergyPlus.Query'></a>

## Query Class

```csharp
public static class Query
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Query
### Methods

<a name='DiGi.EnergyPlus.Query.ExecutablePath(string)'></a>

## Query\.ExecutablePath\(string\) Method

Gets the path of the EnergyPlus executable from [ExecutableConfiguration](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.FileName.ExecutableConfiguration 'DiGi\.EnergyPlus\.Constants\.FileName\.ExecutableConfiguration') in a directory \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\. The path is a machine setting, so it lives in a git\-ignored `user files` conf and never in code\.

The conf holds `key=value` lines; blank lines and lines starting with `#` are skipped, and the key [ExecutablePathKey](DiGi.EnergyPlus.Constants.md#DiGi.EnergyPlus.Constants.Name.ExecutablePathKey 'DiGi\.EnergyPlus\.Constants\.Name\.ExecutablePathKey') names the executable, for example `ExecutablePath=C:\EnergyPlusV26-2-0\energyplus.exe`.

```csharp
public static string? ExecutablePath(string? directory);
```
#### Parameters

<a name='DiGi.EnergyPlus.Query.ExecutablePath(string).directory'></a>

`directory` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The directory holding the conf, typically a `user files` directory\. This value can be null\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The path of the executable, or null when the directory, the conf, its key or the executable does not exist\.