----------------------------------------------------------------
AesEncryptor

simple AES encryptor C#.
you can edit a key and key size to 128, 192 or 256 bits in AesEncrypt.cs
default key size is 128 bits(16 bytes).

 - encrypt/decrypt a byte array to a byte array.
 - encrypt a supported data type to a string.
 - decrypt a string to a supported data type.

more informations:
http://avoex.com
https://www.facebook.com/avoexgames

----------------------------------------------------------------
How to use AesEncryptor

Just one line to encrypt/decrypt.

Encrypt
	- call AvoEx.AesEncryptor.Encrypt(...) function returns a encrypted string or byte.

Decrypt
	- call AvoEx.AesEncryptor.Decrypt...(...) function returns a decrypted data.


watch video tutorial.
http://youtu.be/g4jzSald9Oo

----------------------------------------------------------------
supported data type

 - byte[]
 - bool
 - char
 - double
 - float
 - int
 - long
 - short
 - uint
 - ulong
 - ushort


-------------------------------------------------------
version history

1.0.44
 - supporting Window phone 8.

1.0.39
 - added Decrypt...(byte[]) functions.

1.0.25
 - added readme.txt
 
1.0.20
 - added EncryptKeyIV, DecryptKeyIV function.

1.0.15
 - added ExampleAesEncryptor.
