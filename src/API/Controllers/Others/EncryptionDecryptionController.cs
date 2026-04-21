using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Others
{

    [ApiController]
    public class EncryptionDecryptionController : ControllerBase
    {

        //[HttpPost]
        //[Route("encrypt")]
        //public IActionResult EncryptText([FromBody] string plainText)
        //{
        //    if (string.IsNullOrEmpty(plainText))
        //    {
        //        return BadRequest("Text to encrypt cannot be empty.");
        //    }

        //    try
        //    {
        //        string encryptedText = Encryptor.EncryptStringAES(plainText);
        //        return Ok(encryptedText);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred while encrypting the text: {ex.Message}");
        //    }
        //}



        //[HttpPost]
        //[Route("decrypt")]
        //public IActionResult DecryptText([FromBody] string encryptedText)
        //{
        //    if (string.IsNullOrEmpty(encryptedText))
        //    {
        //        return BadRequest("Encrypted text cannot be empty.");
        //    }

        //    try
        //    {
        //        string decryptedText = Decryptor.DecryptStringAES(encryptedText);
        //        return Ok(decryptedText);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred while decrypting the text: {ex.Message}");
        //    }
        //}



    }
}
