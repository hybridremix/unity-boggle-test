using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Boggle
{
    public class GridInterface : MonoBehaviour
    {
        [HideInInspector] public BoggleSet          m_boggleSet;
        [HideInInspector] public GameObject         m_overlayPanel;
        [HideInInspector] public GridLayoutGroup    m_gridLayout;
        [HideInInspector] public Text[]             m_gridCells;
        [HideInInspector] public Text               m_gameDesc;
        [HideInInspector] public string             m_loading;
        [HideInInspector] public string             m_calculating;
        [HideInInspector] public Button             m_btnSolve;
        [HideInInspector] public Button             m_btnShake;

        public GridInterface() { }

        private void SolveOnClick()
        {
            if (!m_boggleSet.m_isSolved)
            {
                m_overlayPanel.GetComponentInChildren<Text>().text = m_calculating;
                m_overlayPanel.SetActive(true);
                List<string> combos = new List<string>();
                m_boggleSet.CompileWords(combos);

                SetGameDescription(true);
                m_boggleSet.m_isSolved = true;
            }
            m_overlayPanel.SetActive(false);
        }
        private void ShakeOnClick()
        {
            SetGameDescription();
            m_boggleSet.m_solutionCount = 0;
            m_boggleSet.m_solutionPoints = 0;
            m_boggleSet.m_isSolved = false;

            m_boggleSet.m_blocks.ShakeBlocks();
            UpdateGrid(m_boggleSet.m_blocks);
        }
        private void SetGameDescription()
        {
            m_gameDesc.text = "Press SOLVE to find all the possible words!\nPress SHAKE to get a new set of letters.";
        }
        private void SetGameDescription(bool needsReport)
        {
            if (needsReport)
            {
                m_gameDesc.text += ("\nThis grid contains " + m_boggleSet.m_solutionCount.ToString() + " words for a total of " + m_boggleSet.m_solutionPoints.ToString() + " points!");
            }
        }

        public void UpdateGrid(BlockSet grid)
        {
            int c = 0;
            for (int a = 0; a < grid.m_blockGrid.GetLength(0); a++)
            {
                for (int b = 0; b < grid.m_blockGrid.GetLength(1); b++)
                {
                    if (c < m_gridCells.Length)
                    {
                        m_gridCells[c].text = (grid.m_blockGrid[a, b].m_top == 'Q') ? "QU" : grid.m_blockGrid[a, b].m_top.ToString();
                        c++;
                    }
                }
            }
        }
        void Awake()
        {
            m_boggleSet = new BoggleSet("Assets/collins_scrabble_words.txt");
            //m_boggleSet.SetProvidedBoard(true);         //DEBUG
            m_boggleSet.m_blocks.ShakeBlocks();
        }
        // Start is called before the first frame update
        void Start()
        {
            m_calculating = "Counting your words.\nPlease wait...";
            m_loading = "Loading dictionary.\nPlease wait...";

            m_gridLayout = GetComponentInChildren<GridLayoutGroup>();
            m_gridLayout.constraintCount = BoggleGlobals.g_iGridSize;
            m_gridLayout.cellSize = new Vector2((100 / (33.0f * BoggleGlobals.g_iGridSize)) * 100, (100 / (33.0f * BoggleGlobals.g_iGridSize)) * 100);
            m_gridLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (BoggleGlobals.g_iGridSize * m_gridLayout.cellSize.x) + (BoggleGlobals.g_iGridSize * 10.0f));
            m_gridLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (BoggleGlobals.g_iGridSize * m_gridLayout.cellSize.y) + (BoggleGlobals.g_iGridSize * 10.0f));
            GameObject cell0 = m_gridLayout.transform.Find("cell0").gameObject;
            GameObject[] txtObjects = new GameObject[BoggleGlobals.g_iGridSize * BoggleGlobals.g_iGridSize];
            for (int t = 0; t < txtObjects.Length; t++)
            {
                txtObjects[t] = (t == 0) ? cell0 : Instantiate(cell0);
                txtObjects[t].name = "cell" + t.ToString();
                txtObjects[t].GetComponent<Text>().text = "New Cell";
                txtObjects[t].transform.SetParent(m_gridLayout.transform);
                txtObjects[t].transform.localScale = Vector3.one;
                txtObjects[t].transform.localPosition = Vector3.zero;
                txtObjects[t].GetComponentInChildren<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_gridLayout.cellSize.x);
                txtObjects[t].GetComponentInChildren<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_gridLayout.cellSize.y);
            }
            m_gridCells = m_gridLayout.GetComponentsInChildren<Text>();

            m_overlayPanel = GameObject.Find("overlay");
            m_overlayPanel.GetComponentInChildren<Text>().text = m_loading;
            m_overlayPanel.SetActive(false);

            m_gameDesc = GameObject.Find("txt_report").GetComponent<Text>();
            SetGameDescription();
            m_btnSolve = GameObject.Find("btn_solve").GetComponent<Button>();
            m_btnShake = GameObject.Find("btn_shake").GetComponent<Button>();

            UpdateGrid(m_boggleSet.m_blocks);

            m_btnSolve.onClick.AddListener(SolveOnClick);
            m_btnShake.onClick.AddListener(ShakeOnClick);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
