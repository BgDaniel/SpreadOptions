using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Helpers
{
    public class TriangularMatrixSolver
    {
        /*
        Matrix
        1   A   B
        0   C   D
        0   0   E

        Inverse
        1   F   G
        0   H   I
        0   0   J

        where
        F = -A * C^-1
        G = A * C^-1 * D * E^-1 - B * E^-1
        H = C^-1
        D = - C^-1 * D * E^-1
        J = E^-1
        */

        private Matrix<double> m_A;
        private Matrix<double> m_B;
        private Matrix<double> m_C;
        private Matrix<double> m_D;
        private Matrix<double> m_E;

        private Matrix<double> m_F;
        private Matrix<double> m_G;
        private Matrix<double> m_H;
        private Matrix<double> m_I;
        private Matrix<double> m_J;

        private int m_dimC;
        private int m_dimE;
        private int m_nb0;

        public TriangularMatrixSolver(Matrix<double> A, Matrix<double> B, Matrix<double> C, Matrix<double> D,
            Matrix<double> E, int nb0)
        {
            m_A = A;
            m_B = B;
            m_C = C;
            m_D = D;
            m_E = E;

            if (C.ColumnCount != C.RowCount)
                throw new Exception("Matrix C must be square!");

            if (E.ColumnCount != E.RowCount)
                throw new Exception("Matrix C must be square!");

            m_dimC = C.ColumnCount;
            m_dimE = E.ColumnCount;
            m_nb0 = nb0;
            
            var CInv = C.Inverse();
            var EInv = E.Inverse();

            m_F = - m_A * CInv;
            m_G = m_A * CInv * D * EInv - m_B * EInv;
            m_H = CInv;
            m_I = -CInv * D * EInv;
            m_J = EInv;
        }

        public double[] Solve(double[] y)
        {
            double[] _y_0 = new double[m_nb0];
            double[] _y_1 = new double[m_dimC];
            double[] _y_2 = new double[m_dimE];

            Array.Copy(y, 0, _y_0, 0, m_nb0);
            Array.Copy(y, m_nb0, _y_1, 0, m_dimC);
            Array.Copy(y, m_nb0 + m_dimC, _y_2, 0, m_dimE);

            var y_0 = Vector<double>.Build.DenseOfArray(_y_0);
            var y_1 = Vector<double>.Build.DenseOfArray(_y_1);
            var y_2 = Vector<double>.Build.DenseOfArray(_y_2);

            var x_0 = (y_0 + m_F * y_1 + m_G * y_2).ToArray();
            var x_1 = (m_H * y_1 + m_I * y_2).ToArray();
            var x_2 = (m_J * y_2).ToArray();

            var _x = new double[m_nb0 + m_dimC + m_dimE];

            Array.Copy(x_0, 0, _x, 0, m_nb0);
            Array.Copy(x_1, 0, _x, m_nb0, m_dimC);
            Array.Copy(x_2, 0, _x, m_nb0 + m_dimC, m_dimE);

            return _x;
        }

        public double[] Map(double[] x)
        {
            double[] _x_0 = new double[m_nb0];
            double[] _x_1 = new double[m_dimC];
            double[] _x_2 = new double[m_dimE];

            Array.Copy(x, 0, _x_0, 0, m_nb0);
            Array.Copy(x, m_nb0, _x_1, 0, m_dimC);
            Array.Copy(x, m_nb0 + m_dimC, _x_2, 0, m_dimE);

            var x_0 = Vector<double>.Build.DenseOfArray(_x_0);
            var x_1 = Vector<double>.Build.DenseOfArray(_x_1);
            var x_2 = Vector<double>.Build.DenseOfArray(_x_2);

            var y_0 = (x_0 + m_A * x_1 + m_B * x_2).ToArray();
            var y_1 = (m_C * x_1 + m_D * x_2).ToArray();
            var y_2 = (m_E * x_2).ToArray();

            var _y = new double[m_nb0 + m_dimC + m_dimE];

            Array.Copy(y_0, 0, _y, 0, m_nb0);
            Array.Copy(y_1, 0, _y, m_nb0, m_dimC);
            Array.Copy(y_2, 0, _y, m_nb0 + m_dimC, m_dimE);

            return _y;
        }
    }
}
