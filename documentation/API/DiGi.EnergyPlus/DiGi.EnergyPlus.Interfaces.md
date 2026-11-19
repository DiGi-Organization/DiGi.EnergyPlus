#### [DiGi\.EnergyPlus](DiGi.EnergyPlus.Overview.md 'DiGi\.EnergyPlus\.Overview')

## DiGi\.EnergyPlus\.Interfaces Namespace
### Interfaces

<a name='DiGi.EnergyPlus.Interfaces.IEnergyPlusObject'></a>

## IEnergyPlusObject Interface

Base interface for the objects of the EnergyPlus cross\-check \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\.

```csharp
public interface IEnergyPlusObject : DiGi.Core.Interfaces.IObject
```

Derived  
↳ [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult')  
↳ [IEnergyPlusSerializableObject](DiGi.EnergyPlus.Interfaces.md#DiGi.EnergyPlus.Interfaces.IEnergyPlusSerializableObject 'DiGi\.EnergyPlus\.Interfaces\.IEnergyPlusSerializableObject')

Implements [DiGi\.Core\.Interfaces\.IObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iobject 'DiGi\.Core\.Interfaces\.IObject')

<a name='DiGi.EnergyPlus.Interfaces.IEnergyPlusSerializableObject'></a>

## IEnergyPlusSerializableObject Interface

Base interface for the serializable objects of the EnergyPlus cross\-check \(DiGi\-Organization/DiGi\.EnergyPlus\#1\)\.

```csharp
public interface IEnergyPlusSerializableObject : DiGi.EnergyPlus.Interfaces.IEnergyPlusObject, DiGi.Core.Interfaces.IObject, DiGi.Core.Interfaces.ISerializableObject, DiGi.Core.Interfaces.ICloneableObject<DiGi.Core.Interfaces.ISerializableObject>, DiGi.Core.Interfaces.ICloneableObject
```

Derived  
↳ [EnergyPlusResult](DiGi.EnergyPlus.Classes.md#DiGi.EnergyPlus.Classes.EnergyPlusResult 'DiGi\.EnergyPlus\.Classes\.EnergyPlusResult')

Implements [IEnergyPlusObject](DiGi.EnergyPlus.Interfaces.md#DiGi.EnergyPlus.Interfaces.IEnergyPlusObject 'DiGi\.EnergyPlus\.Interfaces\.IEnergyPlusObject'), [DiGi\.Core\.Interfaces\.IObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iobject 'DiGi\.Core\.Interfaces\.IObject'), [DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject'), [DiGi\.Core\.Interfaces\.ICloneableObject&lt;](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1 'DiGi\.Core\.Interfaces\.ICloneableObject\`1')[DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1 'DiGi\.Core\.Interfaces\.ICloneableObject\`1'), [DiGi\.Core\.Interfaces\.ICloneableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject 'DiGi\.Core\.Interfaces\.ICloneableObject')