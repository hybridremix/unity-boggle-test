using System.Collections.Generic;

namespace Boggle
{
    public enum Letter { A = 65, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Qu, R, S, T, U, V, W, X, Y, Z };

    // Create blocks that have 6 sides each
    public class AlphabetBlock
    {
        public Letter[] BlockSides;
        public char BlockTop = ' ';
        public int RowPosition = 0;
        public int ColPosition = 0;

        public AlphabetBlock(int numSides)
        {
            BlockSides = new Letter[numSides];
            for (int i = 0; i < BlockSides.Length; i++)
                BlockSides[i] = 0;
        }
    }

    public class BlockSet
    {
        public AlphabetBlock[,] BlockGrid = new AlphabetBlock[BoggleGlobals.GridSize, BoggleGlobals.GridSize];
        public Dictionary<Letter, int> LetterRepo = new Dictionary<Letter, int>();

        public BlockSet()
        {
            // Inititialize member arrays
            for (int m = 0; m < BlockGrid.GetLength(0); m++)
            {
                for (int n = 0; n < BlockGrid.GetLength(1); n++)
                    BlockGrid[m, n] = new AlphabetBlock(BoggleGlobals.NumBlockSides);
            }

            // Populate the letter repo
            int needed = ((BoggleGlobals.GridSize * BoggleGlobals.GridSize) * BoggleGlobals.NumBlockSides) + BoggleGlobals.NumBlockSides;
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
                        LetterRepo.Add(Letter.E, count);
                        LetterRepo.Add(Letter.T, count);
                        break;
                    case 6:
                        LetterRepo.Add(Letter.A, count);
                        LetterRepo.Add(Letter.R, count);
                        LetterRepo.Add(Letter.I, count);
                        break;
                    case 4:
                        LetterRepo.Add(Letter.N, count);
                        LetterRepo.Add(Letter.O, count);
                        LetterRepo.Add(Letter.S, count);
                        break;
                    case 3:
                        LetterRepo.Add(Letter.D, count);
                        LetterRepo.Add(Letter.C, count);
                        LetterRepo.Add(Letter.L, count);
                        LetterRepo.Add(Letter.F, count);
                        LetterRepo.Add(Letter.M, count);
                        LetterRepo.Add(Letter.P, count);
                        break;
                    case 2:
                        LetterRepo.Add(Letter.Z, count);
                        LetterRepo.Add(Letter.B, count);
                        LetterRepo.Add(Letter.G, count);
                        break;
                    case 1:
                        LetterRepo.Add(Letter.H, count);
                        LetterRepo.Add(Letter.U, count);
                        LetterRepo.Add(Letter.Y, count);
                        LetterRepo.Add(Letter.W, count);
                        LetterRepo.Add(Letter.J, count);
                        LetterRepo.Add(Letter.K, count);
                        LetterRepo.Add(Letter.Qu, count);
                        LetterRepo.Add(Letter.V, count);
                        LetterRepo.Add(Letter.X, count);
                        break;

                    default:
                        break;
                }
            }

            // Pull letters from the repo onto each side of each block
            for (int i = 0; i < BlockGrid.GetLength(0); i++)
            {
                for (int j = 0; j < BlockGrid.GetLength(1); j++)
                {
                    BlockGrid[i, j].RowPosition = i;
                    BlockGrid[i, j].ColPosition = j;
                    //For each block in the grid, fill each side of that block
                    for (int k = 0; k < BoggleGlobals.NumBlockSides; k++)
                    {
                        //Randomly generate a letter between A and Z
                        Letter randKey = (Letter)UnityEngine.Random.Range((int)Letter.A, (int)Letter.Z);
                        if (LetterRepo[randKey] > 0)
                        {
                            // If the value at our current key is nonzero, then it hasn't used up its distribution count...
                            // ...so add our current key to the current side of the block and decrement the key's distribution count.
                            BlockGrid[i, j].BlockSides[k] = randKey;
                            LetterRepo[randKey]--;
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

        public void ShakeBlocks()
        {
            // Randomly select one of the block sides as its top-facing side
            AlphabetBlock block;
            int length;
            for (int r = 0; r < BlockGrid.GetLength(0); r++)
            {
                for (int c = 0; c < BlockGrid.GetLength(1); c++)
                {
                    block = BlockGrid[r, c];
                    length = block.BlockSides.Length;
                    for (int s = 0; s < length; s++)
                    {
                        block.BlockTop = (char)(block.BlockSides[UnityEngine.Random.Range(0, length)]);
                    }
                }
            }
        }
    }
}
