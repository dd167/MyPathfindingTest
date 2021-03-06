﻿using System;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Pathfinding
{

    public static class Utils
    {
        public static readonly double Sqrt2 = Math.Sqrt(2);
        public static Vector2Int InvalidGrid = new Vector2Int(-1, -1);
        public const int ObstacleLayer = 8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Heuristic_Manhattan(int absDX, int absDY)
        {
            return absDX + absDY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Heuristic_Diagonal(int absDX, int absDY)
        {
            return (absDX + absDY) + (Sqrt2 - 2) * Math.Min(absDX, absDY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Heuristic_Euclidean(int absDX, int absDY)
        {
            return Math.Sqrt(absDX * absDX + absDY * absDY);
        }

        /// <summary>
        /// 二维数组转成一维数组
        /// </summary>
        /// <param name="arr2d"></param>
        /// <returns></returns>
        public static byte[] Array2DTo1D(byte[,] arr2d)
        {
            int arr2d_width = arr2d.GetLength(0);
            int arr2d_height = arr2d.GetLength(1);

            //Debug.LogErrorFormat("X={0},Y={1}", arr2d_width, arr2d_height);

            byte[] newarr = new byte[arr2d_width * arr2d_height];

            for( int x = 0; x < arr2d_width; ++x)
            {
                for( int z = 0; z < arr2d_height; ++z )
                {
                    newarr[x * arr2d_height + z] = arr2d[x, z];
                }
            }
            return newarr;
        }

        /// <summary>
        /// 一维数组转二维数组
        /// </summary>
        /// <param name="arr1d"></param>
        /// <returns></returns>
        public static byte[,] Array1DTo2D(byte[] arr1d, int width, int height )
        {
            byte[,] array2d = new byte[width, height];
            for( int x = 0; x < width; ++x )
            {
                for( int z = 0; z < height; ++z )
                {
                    array2d[x, z] = arr1d[x * height + z];
                }
            }
            return array2d;
        }

    }
}