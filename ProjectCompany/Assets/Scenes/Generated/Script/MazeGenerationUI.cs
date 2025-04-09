using UnityEngine;
using UnityEngine.UIElements;

public class MazeGenerationUI : MonoBehaviour
{
    public MazeGenerator m_Generator;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get fields
        IntegerField dimensionInteger = root.Q<IntegerField>("DimensionInteger");
        IntegerField levelInteger = root.Q<IntegerField>("LevelInteger");
        IntegerField pathLimitInteger = root.Q<IntegerField>("PathLimitInteger");

        RadioButtonGroup radioGroup = root.Q<RadioButtonGroup>("RoomsRadioButtonGroup");

        Button generate = root.Q<Button>("GenerateButton");

        generate.clicked += () =>
        {
            int width = dimensionInteger.value;
            int depth = dimensionInteger.value;
            int levels = levelInteger.value;
            int pathLimit = pathLimitInteger.value;

            int selectedAlgorithm = radioGroup.value;

            m_Generator.SetParameters(width, depth, levels, pathLimit);
            m_Generator.SetRoomIndex(selectedAlgorithm);
            m_Generator.GenerateNewMaze();
        };
    }
}
