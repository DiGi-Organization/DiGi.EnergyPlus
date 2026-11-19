#### [DiGi\.EnergyPlus](DiGi.EnergyPlus.Overview.md 'DiGi\.EnergyPlus\.Overview')

## DiGi\.EnergyPlus\.Classes Namespace
### Classes

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult'></a>

## EnergyPlusResult Class

Represents the heating need EnergyPlus calculated for a building written by `Convert.ToEnergyPlus_String` \(DiGi\-Organization/DiGi\.EnergyPlus\#1\): the 12 monthly and the annual sensible heating energy of the ideal\-loads air systems, per system and in total, the thermal transmittances EnergyPlus reports for the external surfaces, the EnergyPlus version and the warning and severe error counts of the run\. Created by `Convert.ToDiGi_EnergyPlusResult` from the output of a run; the constructor only assigns and copies\.

```csharp
public class EnergyPlusResult : DiGi.Core.Classes.SerializableResult, DiGi.EnergyPlus.Interfaces.IEnergyPlusSerializableObject, DiGi.EnergyPlus.Interfaces.IEnergyPlusObject, DiGi.Core.Interfaces.IObject, DiGi.Core.Interfaces.ISerializableObject, DiGi.Core.Interfaces.ICloneableObject<DiGi.Core.Interfaces.ISerializableObject>, DiGi.Core.Interfaces.ICloneableObject
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.Core\.Classes\.Object](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.object 'DiGi\.Core\.Classes\.Object') → [DiGi\.Core\.Classes\.SerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.serializableobject 'DiGi\.Core\.Classes\.SerializableObject') → [DiGi\.Core\.Classes\.SerializableResult](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.serializableresult 'DiGi\.Core\.Classes\.SerializableResult') → EnergyPlusResult

Implements [IEnergyPlusSerializableObject](DiGi.EnergyPlus.Interfaces.md#DiGi.EnergyPlus.Interfaces.IEnergyPlusSerializableObject 'DiGi\.EnergyPlus\.Interfaces\.IEnergyPlusSerializableObject'), [IEnergyPlusObject](DiGi.EnergyPlus.Interfaces.md#DiGi.EnergyPlus.Interfaces.IEnergyPlusObject 'DiGi\.EnergyPlus\.Interfaces\.IEnergyPlusObject'), [DiGi\.Core\.Interfaces\.IObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iobject 'DiGi\.Core\.Interfaces\.IObject'), [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject'), [DiGi\.Core\.Interfaces\.ICloneableObject&lt;](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1 'DiGi\.Core\.Interfaces\.ICloneableObject\`1')[DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1 'DiGi\.Core\.Interfaces\.ICloneableObject\`1'), [DiGi\.Core\.Interfaces\.ICloneableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject 'DiGi\.Core\.Interfaces\.ICloneableObject')
### Constructors

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(DiGi.EnergyPlus.Classes.EnergyPlusResult)'></a>

## EnergyPlusResult\(EnergyPlusResult\) Constructor

Initializes a new instance of the [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult') class by copying an existing instance\.

```csharp
public EnergyPlusResult(DiGi.EnergyPlus.Classes.EnergyPlusResult? energyPlusResult);
```
#### Parameters

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(DiGi.EnergyPlus.Classes.EnergyPlusResult).energyPlusResult'></a>

`energyPlusResult` [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult')

The instance to copy\. This value can be null\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int)'></a>

## EnergyPlusResult\(string, double, IEnumerable\<double\>, IDictionary\<string,List\<double\>\>, IDictionary\<string,double\>, int, int\) Constructor

Initializes a new instance of the [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult') class\. The values are assigned and the collections copied; `Convert.ToDiGi_EnergyPlusResult` reads them\.

```csharp
public EnergyPlusResult(string? energyPlusVersion, double heatingEnergy, System.Collections.Generic.IEnumerable<double>? heatingEnergies, System.Collections.Generic.IDictionary<string,System.Collections.Generic.List<double>>? systemHeatingEnergies, System.Collections.Generic.IDictionary<string,double>? thermalTransmittances, int warningCount, int severeErrorCount);
```
#### Parameters

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).energyPlusVersion'></a>

`energyPlusVersion` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The version of EnergyPlus that ran, as it reports it\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).heatingEnergy'></a>

`heatingEnergy` [System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

The annual sensible heating energy of every ideal\-loads air system \[kWh\]\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).heatingEnergies'></a>

`heatingEnergies` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The monthly sensible heating energy of every ideal\-loads air system, January to December \[kWh\]\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).systemHeatingEnergies'></a>

`systemHeatingEnergies` [System\.Collections\.Generic\.IDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')

The monthly sensible heating energy of each ideal\-loads air system, January to December \[kWh\], keyed by its name as EnergyPlus reports it\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).thermalTransmittances'></a>

`thermalTransmittances` [System\.Collections\.Generic\.IDictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2 'System\.Collections\.Generic\.IDictionary\`2')

The thermal transmittance EnergyPlus reports for each external surface \[W/\(m2K\)\]: the U\-factor with film of an opaque surface, the glass U\-factor of a window, keyed by the surface name in lower case\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).warningCount'></a>

`warningCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of warnings of the run\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(string,double,System.Collections.Generic.IEnumerable_double_,System.Collections.Generic.IDictionary_string,System.Collections.Generic.List_double__,System.Collections.Generic.IDictionary_string,double_,int,int).severeErrorCount'></a>

`severeErrorCount` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of severe errors of the run\.

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(System.Text.Json.Nodes.JsonObject)'></a>

## EnergyPlusResult\(JsonObject\) Constructor

Initializes a new instance of the [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult') class from a JSON object\.

```csharp
public EnergyPlusResult(System.Text.Json.Nodes.JsonObject? jsonObject);
```
#### Parameters

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusResult(System.Text.Json.Nodes.JsonObject).jsonObject'></a>

`jsonObject` [System\.Text\.Json\.Nodes\.JsonObject](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject 'System\.Text\.Json\.Nodes\.JsonObject')

The JSON object\. This value can be null\.
### Properties

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.EnergyPlusVersion'></a>

## EnergyPlusResult\.EnergyPlusVersion Property

Gets the version of EnergyPlus that ran, as it reports it \(for example `26.2.0-4bd7a1f26f`\)\.

```csharp
public string? EnergyPlusVersion { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.HeatingEnergies'></a>

## EnergyPlusResult\.HeatingEnergies Property

Gets a copy of the monthly sensible heating energy of every ideal\-loads air system, January to December \[kWh\]\.

```csharp
public System.Collections.Generic.List<double>? HeatingEnergies { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.HeatingEnergy'></a>

## EnergyPlusResult\.HeatingEnergy Property

Gets the annual sensible heating energy of every ideal\-loads air system, the sum of [HeatingEnergies](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult.HeatingEnergies 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult\.HeatingEnergies') \[kWh\]\.

```csharp
public double HeatingEnergy { get; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.SevereErrorCount'></a>

## EnergyPlusResult\.SevereErrorCount Property

Gets the number of severe errors of the run\.

```csharp
public int SevereErrorCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.SystemHeatingEnergies'></a>

## EnergyPlusResult\.SystemHeatingEnergies Property

Gets a copy of the monthly sensible heating energy of each ideal\-loads air system, January to December \[kWh\], keyed by its name as EnergyPlus reports it \(upper case\)\. The building is one zone heated by one system, so there is one entry\.

```csharp
public System.Collections.Generic.Dictionary<string,System.Collections.Generic.List<double>>? SystemHeatingEnergies { get; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.ThermalTransmittances'></a>

## EnergyPlusResult\.ThermalTransmittances Property

Gets a copy of the thermal transmittance EnergyPlus reports for each external surface \[W/\(m2K\)\]: the U\-factor with film of an opaque surface \(EnergyPlus's own film coefficients, not R\_si and R\_se of PN\-EN ISO 6946\) and the glass U\-factor of a window\. Keyed by the surface name in lower case: EnergyPlus reports names upper\-cased, and a surface written by `Convert.ToEnergyPlus_String` is named after its component's identifier in the "D" format\.

```csharp
public System.Collections.Generic.Dictionary<string,double>? ThermalTransmittances { get; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

<a name='DiGi.EnergyPlus.Classes.EnergyPlusResult.WarningCount'></a>

## EnergyPlusResult\.WarningCount Property

Gets the number of warnings of the run\.

```csharp
public int WarningCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')