using DiGi.Core.Interfaces;

namespace DiGi.EnergyPlus.Interfaces
{
    /// <summary>
    /// Base interface for the serializable objects of the EnergyPlus cross-check (DiGi-Organization/DiGi.EnergyPlus#1).
    /// </summary>
    public interface IEnergyPlusSerializableObject : IEnergyPlusObject, ISerializableObject
    {
    }
}
