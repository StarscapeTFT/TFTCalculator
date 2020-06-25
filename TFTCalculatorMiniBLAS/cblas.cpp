#include "cblas.h"
#include "openblas/cblas.h"

using namespace TFTCalculator;

// C <-- (alpha)A*B + (beta)C
void CBLAS::dgemm(Order Order, Transpose TransA, Transpose TransB, const int M, const int N, const int K,
    const double alpha, const double* A, const int lda, const double* B, const int ldb, const double beta, double* C, const int ldc)
{
    ::cblas_dgemm((CBLAS_ORDER)Order, (CBLAS_TRANSPOSE)TransA, (CBLAS_TRANSPOSE)TransB, M, N, K, alpha, A, lda, B, ldb, beta, C, ldc);
}

void CBLAS::Multiply(MemoryAlignedMatrix^ A, MemoryAlignedMatrix^ B, MemoryAlignedMatrix^ output)
{
    if (A->Size != B->Size || A->Size != output->Size)
    {
        throw gcnew System::InvalidOperationException("All input and output matrices must be the same size.");
    }

    CBLAS::dgemm(Order::CblasRowMajor, Transpose::CblasNoTrans, Transpose::CblasNoTrans,
        A->Size, B->Size, A->Size, 1, A->Data, A->Size, B->Data, B->Size, 0, output->Data, output->Size
    );
}

void CBLAS::Square(MemoryAlignedMatrix^ input, MemoryAlignedMatrix^ output)
{
    if (input->Size != output->Size)
    {
        throw gcnew System::InvalidOperationException("Input and output matrices must be the same size.");
    }

    dgemm(Order::CblasRowMajor, Transpose::CblasNoTrans, Transpose::CblasNoTrans,
        input->Size, input->Size, input->Size,
        1, input->Data, input->Size, input->Data, input->Size, 0, output->Data, input->Size);
}