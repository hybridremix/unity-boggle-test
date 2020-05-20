using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

namespace Boggle
{
    public enum Letter { A = 65, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Qu, R, S, T, U, V, W, X, Y, Z };

    // Create blocks that have 6 sides each
    public class AlphabetBlock
    {
        public Letter[] m_sides;
        public char m_top = ' ';
        public bool m_visited = false;
        public int m_row = 0;
        public int m_col = 0;

        public AlphabetBlock(int numSides)
        {
            m_sides = new Letter[numSides];
            for (int i = 0; i < m_sides.Length; i++)
            {
                // Initialize each side of the block
                m_sides[i] = 0;
            }
        }
    }

    public class BlockSet
    {
        private bool boardProvided = false;

        public AlphabetBlock[,] m_blockGrid = new AlphabetBlock[BoggleGlobals.g_iGridSize, BoggleGlobals.g_iGridSize];

        // Create the letter repository
        public Dictionary<Letter, int> m_letterRepo = new Dictionary<Letter, int>();

        public BlockSet()
        {
            boardProvided = false;

            // Inititialize member arrays
            for (int m = 0; m < m_blockGrid.GetLength(0); m++)
            {
                for (int n = 0; n < m_blockGrid.GetLength(1); n++)
                {
                    m_blockGrid[m, n] = new AlphabetBlock(BoggleGlobals.g_numBlockSides);
                }
            }

            // Populate the letter repo
            int needed = ((BoggleGlobals.g_iGridSize * BoggleGlobals.g_iGridSize) * BoggleGlobals.g_numBlockSides) + BoggleGlobals.g_numBlockSides;
            float modifier = 0.14f;
            for (int iter = 7; iter > 0; iter--)
            {
                int count = (int)((needed * modifier) + 1.0f);
                modifier -= 0.02f;
                // In a game of Boggle, each letter is printed on a block only so many times...
                // ...so we want to populate our dictionary with each letter's distribution count.
                switch (iter)
                {
                    case 7:
                        m_letterRepo.Add(Letter.E, count);
                        m_letterRepo.Add(Letter.T, count);
                        break;
                    case 6:
                        m_letterRepo.Add(Letter.A, count);
                        m_letterRepo.Add(Letter.R, count);
                        m_letterRepo.Add(Letter.I, count);
                        break;
                    case 4:
                        m_letterRepo.Add(Letter.N, count);
                        m_letterRepo.Add(Letter.O, count);
                        m_letterRepo.Add(Letter.S, count);
                        break;
                    case 3:
                        m_letterRepo.Add(Letter.D, count);
                        m_letterRepo.Add(Letter.C, count);
                        m_letterRepo.Add(Letter.L, count);
                        m_letterRepo.Add(Letter.F, count);
                        m_letterRepo.Add(Letter.M, count);
                        m_letterRepo.Add(Letter.P, count);
                        break;
                    case 2:
                        m_letterRepo.Add(Letter.Z, count);
                        m_letterRepo.Add(Letter.B, count);
                        m_letterRepo.Add(Letter.G, count);
                        break;
                    case 1:
                        m_letterRepo.Add(Letter.H, count);
                        m_letterRepo.Add(Letter.U, count);
                        m_letterRepo.Add(Letter.Y, count);
                        m_letterRepo.Add(Letter.W, count);
                        m_letterRepo.Add(Letter.J, count);
                        m_letterRepo.Add(Letter.K, count);
                        m_letterRepo.Add(Letter.Qu, count);
                        m_letterRepo.Add(Letter.V, count);
                        m_letterRepo.Add(Letter.X, count);
                        break;

                    default:
                        break;
                }
            }
            /*int total = 0;
            foreach (var key in m_letterRepo)
            {
                total += key.Value;
            }
            Debug.Log("Letter Repo total " + total.ToString() + " > " + needed.ToString() + "needed");*/

            // Pull letters from the repo onto each side of each block
            for (int i = 0; i < m_blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < m_blockGrid.GetLength(1); j++)
                {
                    m_blockGrid[i, j].m_row = i;
                    m_blockGrid[i, j].m_col = j;
                    //For each block in the grid, fill each side of that block
                    for (int k = 0; k < BoggleGlobals.g_numBlockSides; k++)
                    {
                        //Randomly generate a letter between A and Z
                        Letter randKey = (Letter)UnityEngine.Random.Range((int)Letter.A, (int)Letter.Z);
                        if (m_letterRepo[randKey] > 0)
                        {
                            // If the value at our current key is nonzero, then it hasn't used up its distribution count...
                            // ...so add our current key to the current side of the block and decrement the key's distribution count.
                            m_blockGrid[i, j].m_sides[k] = randKey;
                            m_letterRepo[randKey]--;
                        }
                        else
                        {
                            // Otherwise the value was zero and this key is no longer available...
                            // ...so roll back the iterator to allow us to try again.
                            k--;
                        }
                    }
                }
            }
        }

        public void SpecifyGrid2D(char[,] providedBoard)
        {
            boardProvided = true;
            m_blockGrid = new AlphabetBlock[providedBoard.GetLength(0), providedBoard.GetLength(1)];
            for (int i = 0; i < m_blockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < m_blockGrid.GetLength(1); j++)
                {
                    m_blockGrid[i, j] = new AlphabetBlock(BoggleGlobals.g_numBlockSides);
                    m_blockGrid[i, j].m_row = i;
                    m_blockGrid[i, j].m_col = j;
                    m_blockGrid[i, j].m_top = providedBoard[i, j];
                }
            }
        }
        public AlphabetBlock[,] GetBlocks2D()
        {
            return m_blockGrid;
        }
        public void ClearProvidedBoard()
        {
            boardProvided = false;
            ShakeBlocks();
        }
        public char[,] GetGrid2DAsChar()
        {
            char[,] charBoard = new char[m_blockGrid.GetLength(0), m_blockGrid.GetLength(1)];
            for (int x = 0; x < charBoard.GetLength(0); x++)
            {
                for (int y = 0; y < charBoard.GetLength(1); y++)
                {
                    charBoard[x, y] = m_blockGrid[x, y].m_top;
                }
            }
            return charBoard;
        }
        public void ResetVisited()
        {
            for (int r = 0; r < m_blockGrid.GetLength(0); r++)
            {
                for (int c = 0; c < m_blockGrid.GetLength(1); c++)
                {
                    m_blockGrid[r, c].m_visited = false;
                }
            }
        }

        public void ShakeBlocks()
        {
            if (!boardProvided)
            {
                // Randomly select one of the block sides as its top-facing side
                AlphabetBlock block;
                int length;
                for (int r = 0; r < m_blockGrid.GetLength(0); r++)
                {
                    for (int c = 0; c < m_blockGrid.GetLength(1); c++)
                    {
                        block = m_blockGrid[r, c];
                        length = block.m_sides.Length;
                        for (int s = 0; s < length; s++)
                        {
                            block.m_top = (char)(block.m_sides[UnityEngine.Random.Range(0, length)]);
                        }
                    }
                }
            }
        }

        // Find all adjacent blocks
        public List<AlphabetBlock> FindNewAdjacent(int x, int y)
        {
            List<AlphabetBlock> adj = new List<AlphabetBlock>();
            for (int dx = -1; dx <= 1; dx++)
            {
                int row = x + dx;
                for (int dy = -1; dy <= 1; dy++)
                {
                    int col = y + dy;
                    if (row >= 0 && row < m_blockGrid.GetLength(0)
                      && col >= 0 && col < m_blockGrid.GetLength(1)
                      && (m_blockGrid[row, col] != m_blockGrid[x, y]))
                    {
                        adj.Add(m_blockGrid[row, col]);
                    }
                }
            }

            return adj;
        }
    }
}
