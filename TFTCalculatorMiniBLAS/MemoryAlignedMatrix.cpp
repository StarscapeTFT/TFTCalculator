#include "MemoryAlignedMatrix.h"
#include <malloc.h>
#include <memory.h>
#include "openblas/cblas.h"

using namespace System;
using namespace TFTCalculator;

MemoryAlignedMatrix::MemoryAlignedMatrix(int size)
    : pMatrix(nullptr), size(size)
{
    pMatrix = (double*)_aligned_malloc(size * size * sizeof(double), 64);

    if (pMatrix == nullptr || errno == ENOMEM)
    {
        // Unable to allocate memory, probably because we ran out. Let our C# caller know.
        throw gcnew OutOfMemoryException();
    }

    memset(pMatrix, 0, size * size * sizeof(double));
}

MemoryAlignedMatrix::MemoryAlignedMatrix(int size, double* data)
    : pMatrix(nullptr), size(size)
{
    pMatrix = (double*)_aligned_malloc(size * size * sizeof(double), 64);

    if (pMatrix == nullptr || errno == ENOMEM)
    {
        // Unable to allocate memory, probably because we ran out. Let our C# caller know.
        throw gcnew OutOfMemoryException();
    }

    memcpy(pMatrix, data, size * size * sizeof(double));
}

MemoryAlignedMatrix::~MemoryAlignedMatrix()
{
    if (pMatrix != nullptr)
        _aligned_free(pMatrix);

    pMatrix = nullptr;
}

MemoryAlignedMatrix::!MemoryAlignedMatrix()
{
    if (pMatrix != nullptr)
        _aligned_free(pMatrix);

    pMatrix = nullptr;
}

MemoryAlignedMatrix^ MemoryAlignedMatrix::Copy()
{
    MemoryAlignedMatrix^ m = gcnew MemoryAlignedMatrix(Size, Data);

    return m;
}

double MemoryAlignedMatrix::default::get(int row, int col)
{
    if (row >= size || col >= size || row < 0 || col < 0)
        throw gcnew IndexOutOfRangeException();

    return pMatrix[row + col * size];
}

void MemoryAlignedMatrix::default::set(int row, int col, double value)
{
    if (row >= size || col >= size || row < 0 || col < 0)
        throw gcnew IndexOutOfRangeException();

    pMatrix[row + col * size] = value;
}