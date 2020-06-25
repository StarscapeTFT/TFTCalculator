#pragma once

namespace TFTCalculator
{
    // Utility class for allocating 64-bit aligned ROW-MAJOR matrices for better performance.
    // If anything looks funny, that's because this is a C++/CLI project and not standard C++.
    public ref class MemoryAlignedMatrix
    {
    private:
        double* pMatrix; // raw pointer to the 64-bit aligned data
        int size;


    // For creating copies.
    MemoryAlignedMatrix(int size, double *data);

    public:
        MemoryAlignedMatrix(int size);

        // Destructor to free raw memory.
        ~MemoryAlignedMatrix();

        // Finalizer to free raw memory.
        !MemoryAlignedMatrix();

        MemoryAlignedMatrix^ Copy();

        property double default[int, int]
        {
            double get(int row, int col);
            void set(int row, int col, double value);
        }

        property int Size
        {
            int get() { return size; }
        }

        property double* Data
        {
            double* get() { return pMatrix; }
        }
    };
}