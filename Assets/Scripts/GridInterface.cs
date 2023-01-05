using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Boggle
{
    public class GridInterface : MonoBehaviour
    {
        [HideInInspector] public BoggleSet          GameBoggleSet;
        [HideInInspector] public GameObject         UiOverlayPanel;
        [HideInInspector] public GridLayoutGroup    UiGridLayout;
        [HideInInspector] public Text[]             UiGridCells;
        [HideInInspector] public Text               UiGameDescription;
        [HideInInspector] public string             UiLoadingMessage;
        [HideInInspector] public string             UiCalculatingMessage;
        [HideInInspector] public Button             UiButtonSolve;
        [HideInInspector] public Button             UiButtonShake;

        public GridInterface() { }

        private void SolveOnClick()
        {
            if (!GameBoggleSet.IsSolved)
            {
                UiOverlayPanel.GetComponentInChildren<Text>().text = UiCalculatingMessage;
                UiOverlayPanel.SetActive(true);

                // TODO

                SetGameDescription(true);
                GameBoggleSet.IsSolved = true;
            }
            UiOverlayPanel.SetActive(false);
        }
        private void ShakeOnClick()
        {
            SetGameDescription();
            GameBoggleSet.SolutionCount = 0;
            GameBoggleSet.SolutionPoints = 0;
            GameBoggleSet.IsSolved = false;

            GameBoggleSet.GameBlocks.ShakeBlocks();
            UpdateGrid(GameBoggleSet.GameBlocks);
        }
        private void SetGameDescription()
        {
            UiGameDescription.text = "Press SOLVE to find all the possible words!\nPress SHAKE to get a new set of letters.";
        }
        private void SetGameDescription(bool isSolutionFound)
        {
            if (isSolutionFound)
                UiGameDescription.text += ("\nThis grid contains " + GameBoggleSet.SolutionCount.ToString() + " words for a total of " + GameBoggleSet.SolutionPoints.ToString() + " points!");
        }

        public void UpdateGrid(BlockSet grid)
        {
            int c = 0;
            for (int a = 0; a < grid.BlockGrid.GetLength(0); a++)
            {
                for (int b = 0; b < grid.BlockGrid.GetLength(1); b++)
                {
                    if (c < UiGridCells.Length)
                    {
                        UiGridCells[c].text = (grid.BlockGrid[a, b].BlockTop == 'Q') ? "QU" : grid.BlockGrid[a, b].BlockTop.ToString();
                        c++;
                    }
                }
            }
        }
        void Awake()
        {
            GameBoggleSet = new BoggleSet("Assets/collins_scrabble_words.txt");
            GameBoggleSet.GameBlocks.ShakeBlocks();
        }
        // Start is called before the first frame update
        void Start()
        {
            UiCalculatingMessage = "Counting your words.\nPlease wait...";
            UiLoadingMessage = "Loading dictionary.\nPlease wait...";

            UiGridLayout = GetComponentInChildren<GridLayoutGroup>();
            UiGridLayout.constraintCount = BoggleGlobals.g_GridSize;
            UiGridLayout.cellSize = new Vector2((100 / (33.0f * BoggleGlobals.g_GridSize)) * 100, (100 / (33.0f * BoggleGlobals.g_GridSize)) * 100);
            UiGridLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (BoggleGlobals.g_GridSize * UiGridLayout.cellSize.x) + (BoggleGlobals.g_GridSize * 10.0f));
            UiGridLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (BoggleGlobals.g_GridSize * UiGridLayout.cellSize.y) + (BoggleGlobals.g_GridSize * 10.0f));
            GameObject cell0 = UiGridLayout.transform.Find("cell0").gameObject;
            GameObject[] txtObjects = new GameObject[BoggleGlobals.g_GridSize * BoggleGlobals.g_GridSize];
            for (int t = 0; t < txtObjects.Length; t++)
            {
                txtObjects[t] = (t == 0) ? cell0 : Instantiate(cell0);
                txtObjects[t].name = "cell" + t.ToString();
                txtObjects[t].GetComponent<Text>().text = "New Cell";
                txtObjects[t].transform.SetParent(UiGridLayout.transform);
                txtObjects[t].transform.localScale = Vector3.one;
                txtObjects[t].transform.localPosition = Vector3.zero;
                txtObjects[t].GetComponentInChildren<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UiGridLayout.cellSize.x);
                txtObjects[t].GetComponentInChildren<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UiGridLayout.cellSize.y);
            }
            UiGridCells = UiGridLayout.GetComponentsInChildren<Text>();

            UiOverlayPanel = GameObject.Find("overlay");
            UiOverlayPanel.GetComponentInChildren<Text>().text = UiLoadingMessage;
            UiOverlayPanel.SetActive(false);

            UiGameDescription = GameObject.Find("txt_report").GetComponent<Text>();
            SetGameDescription();
            UiButtonSolve = GameObject.Find("btn_solve").GetComponent<Button>();
            UiButtonShake = GameObject.Find("btn_shake").GetComponent<Button>();

            UpdateGrid(GameBoggleSet.GameBlocks);

            UiButtonSolve.onClick.AddListener(SolveOnClick);
            UiButtonShake.onClick.AddListener(ShakeOnClick);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                Application.Quit();
        }
    }
}
