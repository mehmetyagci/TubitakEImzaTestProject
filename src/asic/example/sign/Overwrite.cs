﻿using System;
using System.IO;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.signature;
using tr.gov.tubitak.uekae.esya.api.signature.sigpackage;

namespace tr.gov.tubitak.uekae.esya.api.asic.example
{
    /**
     * Adds new signatures to given containers, new extended signature
     * will have the same name with the old one
     */

    public class Overwrite : AsicSampleBase
    {
        [Test]
        public void update_CAdES()
        {
            SignaturePackage sp = read(PackageType.ASiC_E, SignatureFormat.CAdES, SignatureType.ES_BES);
            string filename = fileName(PackageType.ASiC_E, SignatureFormat.CAdES, SignatureType.ES_BES) +
                              "-upgraded.asice";
            FileInfo toUpgrade = new FileInfo(filename);

            // create a copy to update package
            sp.write(new FileStream(filename, FileMode.Create));

            // add signature
            SignaturePackage sp2 = SignaturePackageFactory.readPackage(createContext(), toUpgrade);
            SignatureContainer sc = sp2.createContainer();
            Signature s = sc.createSignature(getCertificate());
            s.addContent(sp.getDatas()[0], false);
            s.sign(getSigner());

            // write on to read file!
            sp2.write();

            // read again to verify
            SignaturePackage sp3 = SignaturePackageFactory.readPackage(createContext(), new FileInfo(filename));
            PackageValidationResult pvr = sp3.verifyAll();
            Console.WriteLine(pvr);

            Assert.True(pvr.getResultType() == PackageValidationResultType.ALL_VALID);
        }
    }
}