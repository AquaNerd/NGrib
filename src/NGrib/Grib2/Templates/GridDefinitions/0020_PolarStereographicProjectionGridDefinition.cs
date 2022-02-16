/*
 * This file is part of NGrib.
 *
 * Copyright © 2020 Nicolas Mangué
 * 
 * Copyright 2006-2010 Seaware AB, PO Box 1244, SE-131 28 
 * Nacka Strand, Sweden, info@seaware.se.
 * 
 * Copyright 1997-2006 Unidata Program Center/University 
 * Corporation for Atmospheric Research, P.O. Box 3000, Boulder, CO 80307,
 * support@unidata.ucar.edu.
 * 
 * NGrib is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 * 
 * NGrib is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with NGrib.  If not, see <https://www.gnu.org/licenses/>.
 */

using NGrib.Grib2.CodeTables;
using System;
using System.Collections.Generic;

namespace NGrib.Grib2.Templates.GridDefinitions
{
    public class PolarStereographicProjectionGridDefinition : GridPointEarthGridDefinition
    {
        /// <summary> .</summary>
        /// <returns> Lad as a float
        /// 
        /// </returns>
        public double Lad { get; }

        /// <summary> .</summary>
        /// <returns> Lov as a float
        /// 
        /// </returns>
        public double Lov { get; }

        /// <summary> Get x-increment/distance between two grid points.
        /// 
        /// </summary>
        /// <returns> x-increment
        /// </returns>
        public double Dx { get; }

        /// <summary> Get y-increment/distance between two grid points.
        /// 
        /// </summary>
        /// <returns> y-increment
        /// </returns>
        public double Dy { get; }

        /// <summary> .</summary>
        /// <returns> ProjectionCenter as a int
        /// 
        /// </returns>
        public int ProjectionCenter { get; }

        /// <summary> Get scan mode.
        /// 
        /// </summary>
        /// <returns> scan mode
        /// </returns>
        public int ScanMode { get; }

        internal PolarStereographicProjectionGridDefinition(BufferedBinaryReader reader) : base(reader)
        {
            Lad = reader.ReadInt32() * 1e-6;
            Lov = reader.ReadUInt32() * 1e-6;
            Dx = reader.ReadUInt32() * 1e-3;
            Dy = reader.ReadUInt32() * 1e-3;
            ProjectionCenter = reader.ReadUInt8();
            ScanMode = reader.ReadUInt8();
        }

        public override IEnumerable<Coordinate> EnumerateGridPoints()
        {
            var northPole = ProjectionCenter == 0;
            var latOrigin = northPole ? 90.0 : -90.0;

            // Why the scale factor?. according to GRIB docs:
            // "Grid lengths are in units of meters, at the 60 degree latitude circle nearest to the pole"
            // since the scale factor at 60 degrees = k = 2*k0/(1+sin(60)) [Snyder,Working Manual p157]
            // then to make scale = 1 at 60 degrees, k0 = (1+sin(60))/2 = .933
            double scale;
            // -9999.0 means use a default? Probably an encoding mistake fixed in code.
            if (Lad == default(double))
            {
                scale = 0.9330127018922193;
            }
            else
            {
                scale = (1.0 + Math.Sin(DegreeToRadian(Math.Abs(Math.Abs(Lad)))));
            }

            Projection projection;

            if (IsEarthSpherical(EarthShape.Shape)) {

                throw new NotImplementedException("Still working to finish out work that had been done in the netcdf java lib. " +
                    "https://github.com/Unidata/netcdf-java/blob/cbe43a8c40520ae59782727753080bf6469ebb5d/grib/src/main/java/ucar/nc2/grib/grib2/Grib2Gds.java#L983");

            }

        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private class Projection
        {

        }

        private bool IsEarthSpherical(EarthShape earthShape)
        {
            var isShperical = earthShape == CodeTables.EarthShape.CustomSpherical;
            return isShperical;
        }
    }
}