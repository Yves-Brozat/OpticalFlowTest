using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace OpticalFlowTest {

/// <summary>
/// Binder VFX pour activer/désactiver 8 forces (Conform to Sphere) en fonction des zones de poèmes
/// Chaque force correspond à une zone de lecture de poèmes
/// </summary>
[AddComponentMenu("VFX/Property Binders/Poem Zone Force Binder")]
[VFXBinder("Poem Zone Force")]
public sealed class VFXPoemZoneForceBinder : VFXBinderBase
{
    [Header("VFX Properties")]
    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 1")]
    public ExposedProperty Force1Property = "Force1_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 2")]
    public ExposedProperty Force2Property = "Force2_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 3")]
    public ExposedProperty Force3Property = "Force3_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 4")]
    public ExposedProperty Force4Property = "Force4_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 5")]
    public ExposedProperty Force5Property = "Force5_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 6")]
    public ExposedProperty Force6Property = "Force6_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 7")]
    public ExposedProperty Force7Property = "Force7_Enabled";

    [VFXPropertyBinding("System.Boolean")]
    [Tooltip("Propriété bool pour la force de la zone 8")]
    public ExposedProperty Force8Property = "Force8_Enabled";

    [Header("Target")]
    [Tooltip("Référence au PoemReaderPlaylist qui gère les zones")]
    public PoemReaderPlaylist Target = null;

    private ExposedProperty[] _forceProperties;

    void Awake()
    {
        // Initialiser le tableau des propriétés pour un accès rapide
        _forceProperties = new ExposedProperty[]
        {
            Force1Property,
            Force2Property,
            Force3Property,
            Force4Property,
            Force5Property,
            Force6Property,
            Force7Property,
            Force8Property
        };
    }

    public override bool IsValid(VisualEffect component)
    {
        if (Target == null) return false;

        // Vérifier que toutes les propriétés existent dans le VFX
        for (int i = 0; i < 8; i++)
        {
            if (!component.HasBool(_forceProperties[i]))
                return false;
        }

        return true;
    }

    public override void UpdateBinding(VisualEffect component)
    {
        if (Target == null) return;

        // Mettre à jour chaque force en fonction de l'état de la zone correspondante
        for (int i = 0; i < 8; i++)
        {
            bool forceEnabled = Target.GetForceState(i);
            component.SetBool(_forceProperties[i], forceEnabled);
        }
    }

    public override string ToString()
    {
        return $"Poem Zone Forces : 8 forces -> " +
               (Target != null ? Target.name : "(null)");
    }
}

} // namespace OpticalFlowTest
