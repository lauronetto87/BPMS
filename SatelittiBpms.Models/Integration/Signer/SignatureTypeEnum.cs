namespace SatelittiBpms.Models.Integration.Signer
{
    /// <summary> 
    /// Signature type: 
    /// - 0: Eletronic or Digital 
    /// - 1: Digital 
    /// - 2: Eletronic 
    /// - 3: Inperson 
    /// </summary> 
    public enum SignatureTypeEnum
    {
        BOTH = 0,
        DIGITAL = 1,
        ELETRONIC = 2,
        INPERSON = 3
    }
}
