using UnityEngine;
using TMPro;

public class MetadataUIController : MonoBehaviour {
    public TextMeshProUGUI damagePointsText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI savedVictimsText;
    public TextMeshProUGUI stepsText;

    // Método para actualizar la UI
    public void UpdateMetadata(Metadata metadata) {
        damagePointsText.text = $"Damage Points: {metadata.damage_points}";
        deathsText.text = $"Deaths: {metadata.deaths}";
        savedVictimsText.text = $"Saved Victims: {metadata.saved_victims}";
        stepsText.text = $"Steps: {metadata.steps}";
    }
}
