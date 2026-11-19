#### [DiGi\.EnergyPlus](DiGi.EnergyPlus.Overview.md 'DiGi\.EnergyPlus\.Overview')

## DiGi\.EnergyPlus\.Constants Namespace
### Classes

<a name='DiGi.EnergyPlus.Constants.Default'></a>

## Default Class

Default values of the EnergyPlus export \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\.

```csharp
public static class Default
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Default
### Fields

<a name='DiGi.EnergyPlus.Constants.Default.MaximumLayerCount'></a>

## Default\.MaximumLayerCount Field

The largest number of layers of an EnergyPlus `Construction`\.

```csharp
public const int MaximumLayerCount = 10;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.EnergyPlus.Constants.Default.MaximumWarmupDays'></a>

## Default\.MaximumWarmupDays Field

The maximum number of warm\-up days\. A heavy building on a constant set\-point converges slowly, so the EnergyPlus default of 25 is raised\.

```csharp
public const int MaximumWarmupDays = 50;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.EnergyPlus.Constants.Default.MinimumThermalResistance'></a>

## Default\.MinimumThermalResistance Field

The smallest thermal resistance EnergyPlus accepts for a `Material:NoMass` \[m2K/W\]\.

```csharp
public const double MinimumThermalResistance = 0.001;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Default.MinimumWarmupDays'></a>

## Default\.MinimumWarmupDays Field

The minimum number of warm\-up days\.

```csharp
public const int MinimumWarmupDays = 6;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.EnergyPlus.Constants.Default.PlaceholderThermalResistance'></a>

## Default\.PlaceholderThermalResistance Field

The thermal resistance of the placeholder construction of an adiabatic surface whose own construction cannot be written \[m2K/W\]\.

```csharp
public const double PlaceholderThermalResistance = 0.1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Default.Roughness'></a>

## Default\.Roughness Field

The roughness of every material written\.

```csharp
public const string Roughness = "MediumRough";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Default.ThermalAbsorptance'></a>

## Default\.ThermalAbsorptance Field

The thermal absorptance \(emissivity\) of every material written \[\-\]\.

```csharp
public const double ThermalAbsorptance = 0.9;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Default.ThermalBridgeOffset'></a>

## Default\.ThermalBridgeOffset Field

The distance between the building and the hidden surface that carries the thermal bridges \[m\]\.

```csharp
public const double ThermalBridgeOffset = 1;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Default.ThermalBridgeThermalTransmittance'></a>

## Default\.ThermalBridgeThermalTransmittance Field

The thermal transmittance of the hidden surface that carries the thermal bridges \[W/\(m2K\)\]\. Its area is H\_tb divided by this value; a low value keeps the surface's own film resistance, which EnergyPlus calculates rather than takes as R\_si, a small part of its resistance\.

```csharp
public const double ThermalBridgeThermalTransmittance = 0.25;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Default.TimestepsPerHour'></a>

## Default\.TimestepsPerHour Field

The number of time steps per hour of the simulation\.

```csharp
public const int TimestepsPerHour = 6;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.EnergyPlus.Constants.Default.Version'></a>

## Default\.Version Field

The EnergyPlus version the input file is written for, as its `Version` object states it\. The field lists of every object written were checked against the IDD of this release\.

```csharp
public const string Version = "26.2";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Default.VisibleAbsorptance'></a>

## Default\.VisibleAbsorptance Field

The visible absorptance of every material written \[\-\]\.

```csharp
public const double VisibleAbsorptance = 0.7;
```

#### Field Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Constants.Name'></a>

## Name Class

Names of the objects of the EnergyPlus input file that a reader of its output refers to \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\.

```csharp
public static class Name
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Name
### Fields

<a name='DiGi.EnergyPlus.Constants.Name.GroundOtherSideCoefficients'></a>

## Name\.GroundOtherSideCoefficients Field

The name of the other\-side coefficients that hold the outside face of a floor on the ground or over the cellar at the monthly mean external temperature\.

```csharp
public const string GroundOtherSideCoefficients = "Ground";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Name.IdealLoadsAirSystem'></a>

## Name\.IdealLoadsAirSystem Field

The name of the ideal\-loads air system that heats the zone\.

```csharp
public const string IdealLoadsAirSystem = "Ideal Loads";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Name.ThermalBridgeOtherSideCoefficients'></a>

## Name\.ThermalBridgeOtherSideCoefficients Field

The name of the other\-side coefficients that hold the outside face of the thermal\-bridge surface at the outdoor dry\-bulb temperature\.

```csharp
public const string ThermalBridgeOtherSideCoefficients = "Thermal Bridges";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Name.ThermalBridgeSurface'></a>

## Name\.ThermalBridgeSurface Field

The name of the hidden surface that carries the thermal bridges\.

```csharp
public const string ThermalBridgeSurface = "Thermal Bridges";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Constants.Name.Zone'></a>

## Name\.Zone Field

The name of the single zone the building is modelled as\.

```csharp
public const string Zone = "Building";
```

#### Field Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')