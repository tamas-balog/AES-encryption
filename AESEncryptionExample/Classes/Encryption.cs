using System;
using System.Collections.Generic;
using System.Text;

namespace AESEncryptionExample.Classes
{
    public class Encryption
    {
        public string Text { get; set; }

        public string EncryptionKey { get; set; }

        public List<StateMatrix> RoundKeys { get; set; }

        public List<List<Round>> TextRounds { get; set; }

        public List<List<Round>> InvRounds { get; set; }

        public Encryption(string text, string key)
        {
            EncryptionKey = key;
            Text = text;
            TextRounds = new List<List<Round>>();
            InvRounds = new List<List<Round>>();

            GenerateKey();
            List<StateMatrix> cipher = Encrypt();
            string decipher = Decrypt(cipher);
        }

        public List<StateMatrix> Encrypt()
        {
            if (Text.Length % 16 != 0)
            {
                int padding = -(Text.Length % 16 - 16);
                Text = Text.PadRight(Text.Length + padding, (char)0);
            }

            List<StateMatrix> matrixBlocks = new List<StateMatrix>();
            for (int i = 0, j = 0; i < Text.Length / 16; i++, j += 16)
            {
                matrixBlocks.Add(new StateMatrix(Text.Substring(j, 16)));
            }

            List<StateMatrix> result = new List<StateMatrix>();
            foreach (StateMatrix matrix in matrixBlocks)
            {
                List<Round> rounds = new List<Round>();
                Round round = new Round();
                round.Matrixes.Add(matrix.RoundsToString("Original"));
                StateMatrix state = AddRoundKey(matrix, 0);
                round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                rounds.Add(round);
                TextRounds.Add(rounds);
                for (int i = 1; i <= 9; i++)
                {
                    rounds = new List<Round>();
                    round = new Round();
                    state = SubBytes(state);
                    round.Matrixes.Add(state.RoundsToString("SubBytes"));
                    state = ShiftRows(state);
                    round.Matrixes.Add(state.RoundsToString("ShiftRows"));
                    state = MixColumns(state);
                    round.Matrixes.Add(state.RoundsToString("MixColumns"));
                    state = AddRoundKey(state, i);
                    round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                    rounds.Add(round);
                    TextRounds.Add(rounds);
                }

                rounds = new List<Round>();
                round = new Round();
                state = SubBytes(state);
                round.Matrixes.Add(state.RoundsToString("SubBytes"));
                state = ShiftRows(state);
                round.Matrixes.Add(state.RoundsToString("ShiftRows"));
                state = AddRoundKey(state, 10);
                round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                rounds.Add(round);
                TextRounds.Add(rounds);
                result.Add(state);
            }

            return result;
        }

        public string Decrypt(List<StateMatrix> stateMatrix)
        {
            string result = "";
            foreach (StateMatrix matrix in stateMatrix)
            {
                List<Round> rounds = new List<Round>();
                Round round = new Round();
                round.Matrixes.Add(matrix.RoundsToString("Original"));
                StateMatrix state = AddRoundKey(matrix, 10);
                round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                rounds.Add(round);
                InvRounds.Add(rounds);
                for (int i = 1; i <= 9; i++)
                {
                    rounds = new List<Round>();
                    round = new Round();
                    state = InvShiftRows(state);
                    round.Matrixes.Add(state.RoundsToString("ShiftRows"));
                    state = InvSubBytes(state);
                    round.Matrixes.Add(state.RoundsToString("SubBytes"));
                    state = AddRoundKey(state, 10 - i);
                    round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                    state = InvMixColumns(state);
                    round.Matrixes.Add(state.RoundsToString("MixColumns"));
                    rounds.Add(round);
                    InvRounds.Add(rounds);
                }

                rounds = new List<Round>();
                round = new Round();
                state = InvShiftRows(state);
                round.Matrixes.Add(state.RoundsToString("ShiftRows"));
                state = InvSubBytes(state);
                round.Matrixes.Add(state.RoundsToString("SubBytes"));
                state = AddRoundKey(state, 0);
                round.Matrixes.Add(state.RoundsToString("AddRoundKey"));
                rounds.Add(round);
                InvRounds.Add(rounds);
                result += state.ToString();
            }
            return result;
        }

        public void RotWord(List<FiniteField> word)
        {
            FiniteField temp = word[0];
            for (int i = 0; i < word.Count - 1; i++)
            {
                word[i] = word[i + 1];
            }
            word[word.Count - 1] = temp;
        }

        public void SubWord(List<FiniteField> word)
        {
            for (int i = 0; i < word.Count; i++)
            {
                word[i] = FiniteField.SBox(word[i]);
            }
        }

        public void Rcon(ref FiniteField roundConstant, int round)
        {
            if (round == 1)
            {
                return;
            }
            else if (roundConstant < new FiniteField("10000000"))
            {
                roundConstant *= new FiniteField("00000010");
            }
            else
            {
                roundConstant = roundConstant * new FiniteField("00000010") + new FiniteField("100011011");
            }
        }

        public void GenerateKey()
        {

            RoundKeys = new List<StateMatrix>();
            StateMatrix roundKey = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    roundKey[j, i] = new FiniteField(Convert.ToString(EncryptionKey[i * 4 + j], 2).PadLeft(8, '0'));
                }
            }

            FiniteField roundConstant = new FiniteField("00000001");
            RoundKeys.Add(roundKey);
            for (int round = 1; round <= 10; round++)
            {
                roundKey = new StateMatrix();
                List<FiniteField> word = new List<FiniteField>();
                for (int i = 0; i < 4; i++)
                {
                    word.Add(RoundKeys[round - 1][i, 3]);
                }
                RotWord(word);
                SubWord(word);
                Rcon(ref roundConstant, round);
                word[0] = word[0] + roundConstant;
                for (int i = 0; i < 4; i++)
                {
                    roundKey[i, 0] = RoundKeys[round - 1][i, 0] + word[i];
                }
                for (int i = 1; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        roundKey[j, i] = RoundKeys[round - 1][j, i] + roundKey[j, i - 1];
                    }
                }
                RoundKeys.Add(roundKey);
            }
        }

        public StateMatrix AddRoundKey(StateMatrix a, int round)
        {
            StateMatrix key = RoundKeys[round];
            return a + key;
        }

        public StateMatrix SubBytes(StateMatrix stateMatrix)
        {
            StateMatrix subbedMatrix = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    subbedMatrix[j, i] = FiniteField.SBox(stateMatrix[j, i]);
                }
            }

            return subbedMatrix;
        }

        public StateMatrix InvSubBytes(StateMatrix stateMatrix)
        {
            StateMatrix subbedMatrix = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    subbedMatrix[j, i] = FiniteField.InvSBox(stateMatrix[j, i]);
                }
            }

            return subbedMatrix;
        }

        public StateMatrix ShiftRows(StateMatrix stateMatrix)
        {
            StateMatrix shifted = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                int index = (4 - i) % 4;
                for (int j = 0; j < 4; j++)
                {
                    shifted[i, index] = stateMatrix[i, j];
                    index++;
                    index %= 4;
                }
            }

            return shifted;
        }

        public StateMatrix InvShiftRows(StateMatrix stateMatrix)
        {
            StateMatrix shifted = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                int index = i % 4;
                for (int j = 0; j < 4; j++)
                {
                    shifted[i, index] = stateMatrix[i, j];
                    index++;
                    index %= 4;
                }
            }

            return shifted;
        }

        public StateMatrix MixColumns(StateMatrix stateMatrix)
        {
            StateMatrix mixedMatrix = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    mixedMatrix[i, j] = new FiniteField();
                    for (int k = 0; k < 4; k++)
                    {
                        mixedMatrix[i, j] += stateMatrix[k, j] * StateMatrix.C[i, k];
                    }
                }
            }

            return mixedMatrix;
        }

        public StateMatrix InvMixColumns(StateMatrix stateMatrix)
        {
            StateMatrix mixedMatrix = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    mixedMatrix[i, j] = new FiniteField();
                    for (int k = 0; k < 4; k++)
                    {
                        mixedMatrix[i, j] += stateMatrix[k, j] * StateMatrix.InvC[i, k];
                    }
                }
            }

            return mixedMatrix;
        }

        public List<string> KeysToString()
        {
            List<string> keys = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < RoundKeys.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        stringBuilder.Append(RoundKeys[i][k, j].ToHexaString());
                        stringBuilder.Append(" ");
                    }
                }
                keys.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }

            return keys;
        }
    }
}
