using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.cades.example.validation;
using tr.gov.tubitak.uekae.esya.api.cmssignature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.attribute;
using tr.gov.tubitak.uekae.esya.api.cmssignature.example.util;
using tr.gov.tubitak.uekae.esya.api.cmssignature.signature;
using tr.gov.tubitak.uekae.esya.api.cmssignature.validation;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.asn.util;

namespace tr.gov.tubitak.uekae.esya.api.cades.example.sign
{
    [TestFixture]
    public class ESXLongSign : CadesSampleBase
    {
        private readonly DirectoryInfo testDataDirectory = Directory.CreateDirectory(getTestDataFolder());

        /**
	     * creates ESXLong type signature and validate it.
	     * @throws Exception
	     */

        [Test]
        public void testEsxlongSign()
        {
            BaseSignedData bs = new BaseSignedData();

            ISignable content = new SignableByteArray(Encoding.ASCII.GetBytes("test"));
            bs.addContent(content);

            Dictionary<string, object> params_ = new Dictionary<string, object>();

            //if you are using test certificates,without QCStatement,you must set P_CHECK_QC_STATEMENT to false.
            //By default,it is true
            params_[EParameters.P_VALIDATE_CERTIFICATE_BEFORE_SIGNING] = false;

            bool checkQCStatement = isQualified();
            //necassary for getting signaturetimestamp
            params_[EParameters.P_TSS_INFO] = getTSSettings();

            //necessary for validation of signer certificate according to time in signaturetimestamp attribute
            params_[EParameters.P_CERT_VALIDATION_POLICY] = getPolicy();


            //Get qualified or non-qualified certificate.
            ECertificate cert = SmartCardManager.getInstance().getSignatureCertificate(checkQCStatement);
            BaseSigner signer = SmartCardManager.getInstance().getSigner(getPin(), cert);

            //add signer
            bs.addSigner(ESignatureType.TYPE_ESXLong, cert, signer, null, params_);

            SmartCardManager.getInstance().logout();

            byte[] signedDocument = bs.getEncoded();

            AsnIO.dosyayaz(signedDocument, testDataDirectory.FullName + @"\ESXLong-1.p7s");

            CadesSignatureValidation validationUtil = new CadesSignatureValidation();
            SignedDataValidationResult sdvr = validationUtil.validate(signedDocument, null);

            Assert.AreEqual(SignedData_Status.ALL_VALID, sdvr.getSDStatus());
        }
    }
}