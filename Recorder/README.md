# SampleApp (Sunameri.exe)

## Usage

```ps1
# Record key inputs.
.\Sunameri.exe record --port COM6 | Tee-Object .\sample.json

# Replay the operations.
.\Sunameri.exe replay --port COM6 --input .\sample.json
```

## Key bindings

### Buttons

| Button | Keyboard |
| ------ | -------- |
| A      | X        |
| B      | Z        |
| X      | C        |
| Y      | S        |
| L      | Q        |
| R      | R        |
| Z      | D        |
| Start  | Return   |
| Left   | F        |
| Right  | H        |
| Down   | G        |
| Up     | T        |
| (Exit) | Escape   |

### Control Stick

| Tilt  | Keyboard |
| ----- | -------- |
| Left  | Left     |
| Right | Right    |
| Down  | Down     |
| Up    | Up       |

### C Stick

| Tilt  | Keyboard |
| ----- | -------- |
| Left  | J        |
| Right | L        |
| Down  | K        |
| Up    | I        |
