# csharp-cipher-cli
 
## Description
A command-line interface (**CLI**) tool that allows to encrypt/decrypt data with given **key**

## Usage

### 1st way

Use `cmd` to enter commands

See [examples](examples) folder for more information

```
application.exe --action "action_value" --key "key_value" --file "C:/path/to/file.txt"
```

| Parameter | Value | Description |
| :--- | :--- | :--- |
| `--action` | `encrypt` \| `decrypt` | select what  you want |
| `--key` | _enter yours_  | please dont forget it |
| `--file` | _enter yours_  | path to file (absolute or relative) |

**Encrypt**
```
cipher-cli.exe --action encrypt --key foobar --file "./showcase.json"
```

**Decrypt**
```
cipher-cli.exe --action decrypt --key foobar --file "./showcase-encrypted.txt"
```

### 2nd way

This is kinda password manager tool

Requirements — data stored in **special format**

- Run `.exe` file as regular application
- File dialogue window will popup
- Select file with encrypted contents
- Enter key
- Use arrow keys (<kbd>&#8593;</kbd>, <kbd>&#8595;</kbd>) to navigate in list of entries
- Press <kbd>Enter</kbd> key to select desired entry
- It will show description of the entry and associated data (displayed as asterix as it might be sensitive information)
- Again, use arrow keys to navigate, and select — it will copy that data to clipboard, so you can paste in the future

**Special format**

```json
[
  {
    "name": "some website",
    "description": "email + password",
    "data": ["foobar@email.com", "123456"]
  },
  {
    "name": "another website",
    "description": "email + password",
    "data": ["user@foobar.com", "qwerty123"]
  }
]
```

## How does it works
**Step 1.** 
Ask for text and key

_Example_
- text — `Hello, World!`
- key — `123`

**Step 2.**
Convert given text and key from normal string to array of bytes (in `utf-8` encoding)

_Example_
- text — `[72, 101, 108, 108, 111, 44, 32, 87, 111, 114, 108, 100, 33]`
- key — `[49, 50, 51]`

**Step 3.**
Sequentially `xor` each byte of text with byte of key (in the algorithm key becames normalized to match length)

_Example_
- Normalized key: from `[49, 50, 51]` to `[49, 50, 51, 49, 50, 51, 49, 50, 51, 49, 50, 51, 49]`
- Xoring: `[72 ^ 49, 101 ^ 50, 108 ^ 51, 108 ^ 49, ...]` &#8594; `[121, 87, 95, 93, ...]`


**Step 4.**
Put `xored` byte into new array at the corresponding index

_Example_
- Output: `[121, 87, 95, 93, 93, 31, 17, 101, 92, 67, 94, 87, 16]`

**Step 5.**
Convert array of bytes to string (in `base64` encoding)

_Example_
- Output: `eVdfXV0fEWVcQ15XEA==`

**Step 6.**
Save output to disk or cloud, **DON'T** forget the key, otherwise you could not decrypt file 🤬
