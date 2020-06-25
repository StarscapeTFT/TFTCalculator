#pragma once

#include "MemoryAlignedMatrix.h"

namespace TFTCalculator
{
    public ref class CBLAS abstract sealed
    {
    public:
        enum class Order { CblasRowMajor = 101, CblasColMajor = 102 };
        enum class Transpose { CblasNoTrans = 111, CblasTrans = 112, CblasConjTrans = 113, CblasConjNoTrans = 114 };

        static void dgemm(Order Order, Transpose TransA, Transpose TransB, const int M, const int N, const int K,
            const double alpha, const double* A, const int lda, const double* B, const int ldb, const double beta, double* C, const int ldc);

        static void Multiply(MemoryAlignedMatrix^ A, MemoryAlignedMatrix^ B, MemoryAlignedMatrix^ C);
        static void Square(MemoryAlignedMatrix^ input, MemoryAlignedMatrix^ output);
    };
}