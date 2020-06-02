using System;
using System.Collections.Generic;
using System.Text;

namespace AESEncryptionExample.Classes
{
    public class StateMatrix
    {
        FiniteField[,] matrix;
        public FiniteField[,] Matrix { get => matrix; set => matrix = value; }

        public static StateMatrix C
        {
            get
            {
                StateMatrix constant = new StateMatrix();
                constant.matrix = new FiniteField[4, 4]
                {
                    {
                        new FiniteField("00000010"),
                        new FiniteField("00000011"),
                        new FiniteField("00000001"),
                        new FiniteField("00000001")
                    },
                    {
                        new FiniteField("00000001"),
                        new FiniteField("00000010"),
                        new FiniteField("00000011"),
                        new FiniteField("00000001")
                    },
                    {
                        new FiniteField("00000001"),
                        new FiniteField("00000001"),
                        new FiniteField("00000010"),
                        new FiniteField("00000011")
                    },
                    {
                        new FiniteField("00000011"),
                        new FiniteField("00000001"),
                        new FiniteField("00000001"),
                        new FiniteField("00000010")
                    },
                };
                return constant;
            }
        }

        public static StateMatrix InvC
        {
            get
            {
                StateMatrix constant = new StateMatrix();
                constant.matrix = new FiniteField[4, 4]
                {
                    {
                        new FiniteField("00001110"),
                        new FiniteField("00001011"),
                        new FiniteField("00001101"),
                        new FiniteField("00001001")
                    },
                    {
                        new FiniteField("00001001"),
                        new FiniteField("00001110"),
                        new FiniteField("00001011"),
                        new FiniteField("00001101")
                    },
                    {
                        new FiniteField("00001101"),
                        new FiniteField("00001001"),
                        new FiniteField("00001110"),
                        new FiniteField("00001011")
                    },
                    {
                        new FiniteField("00001011"),
                        new FiniteField("00001101"),
                        new FiniteField("00001001"),
                        new FiniteField("00001110")
                    },
                };
                return constant;
            }
        }

        public StateMatrix()
        {
            matrix = new FiniteField[4, 4];
        }
        public StateMatrix(string word)
        {
            if (word.Length != 16)
            {
                throw new Exception();
            }

            matrix = new FiniteField[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[j, i] = new FiniteField(Convert.ToString(word[i * 4 + j], 2).PadLeft(8, '0'));
                }
            }
        }
        public FiniteField this[int i, int j]
        {
            get { return Matrix[i, j]; }
            set { Matrix[i, j] = value; }
        }

        public static StateMatrix operator +(StateMatrix a, StateMatrix b)
        {
            StateMatrix product = new StateMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    product[i, j] = a[i, j] + b[i, j];
                }
            }
            return product;
        }

        public RoundState RoundsToString(string header)
        {
            List<List<string>> rounds = new List<List<string>>();
            for (int i = 0; i < 4; i++)
            {
                List<string> temp = new List<string>();
                for (int j = 0; j < 4; j++)
                {
                    temp.Add(Matrix[i, j].ToHexaString());
                }
                rounds.Add(temp);
            }

            RoundState roundHelper = new RoundState(header, rounds);

            return roundHelper;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    stringBuilder.Append((char)Convert.ToInt32(matrix[j, i].ToString(), 2) != (char)0 ? ((char)Convert.ToInt32(matrix[j, i].ToString(), 2)).ToString() : "");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
