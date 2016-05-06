﻿using System.Collections.Generic;
using System.IO;
using SequencingFiles;
using Pisces.Domain.Models;
using Pisces.Domain.Models.Alleles;
using Pisces.Domain.Types;
namespace TestUtilities
{
    public static class TestHelper
    {
        public static void SetQualities(AlignmentSet set, int quality)
        {
            for (var i = 0; i < set.PartnerRead1.Qualities.Length; i++)
                set.PartnerRead1.Qualities[i] = (byte)quality;

            if (set.PartnerRead2 != null)
                for (var i = 0; i < set.PartnerRead2.Qualities.Length; i++)
                    set.PartnerRead2.Qualities[i] = (byte)quality;
        }

        public static AlignmentSet CreateTestSet(Read read1, int quality = 30)
        {
            var alignmentSet = new AlignmentSet(read1, null, true);
            SetQualities(alignmentSet, quality);

            return alignmentSet;
        }

        public static AlignmentSet CreateTestSet(Read read1, Read read2, int quality = 30)
        {
            var alignmentSet = new AlignmentSet(read1, read2, true);
            SetQualities(alignmentSet, quality);

            return alignmentSet;
        }

        public static CigarAlignment GetReadCigarFromStitched(string stitchedCigar, int readLength, bool reverse)
        {
            var cigar = new CigarAlignment(stitchedCigar);
            if (reverse)
                cigar.Reverse();

            var totalLengthSofar = 0;
            var newCigar = new CigarAlignment();

            for (var i = 0; i < cigar.Count; i++)
            {
                var operation = cigar[i];
                if (operation.IsReadSpan())
                {
                    if (totalLengthSofar + operation.Length > readLength)
                    {
                        newCigar.Add(new CigarOp(operation.Type, (uint)(readLength - totalLengthSofar)));
                        break;
                    }

                    newCigar.Add(operation);
                    totalLengthSofar += (int)operation.Length;

                    if (totalLengthSofar == readLength)
                        break;
                }
                else
                {
                    newCigar.Add(operation);
                }
            }

            if (reverse)
                newCigar.Reverse();

            return newCigar;
        }



        public static BaseCalledAllele CreatePassingVariant(bool isReference)
        {
            var calledAllele = isReference ? (BaseCalledAllele)new CalledReference() :
                new CalledVariant(AlleleCategory.Snv);

            calledAllele.Coordinate = 1;
            calledAllele.Alternate = "C";
            calledAllele.Reference = "A";
            calledAllele.AlleleSupport = isReference ? 490 : 10;
            calledAllele.TotalCoverage = 490;
            calledAllele.NumNoCalls = 10;
            calledAllele.StrandBiasResults = new StrandBiasResults() { BiasAcceptable = true };
            calledAllele.VariantQscore = 30;

            return calledAllele;
        }

    }
}