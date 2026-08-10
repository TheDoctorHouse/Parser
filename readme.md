# Parser

Parser is a small interpreter written in C#. It tokenizes source code, builds an abstract syntax tree, and evaluates the resulting program. The project is intended as a compact, readable implementation of a simple expression language.

## Build

The project targets .NET 10. Install the .NET 10 SDK, then build the solution from the repository root:

```powershell
dotnet build
```

## Run a program

Use the `interpret` command with the path to a source file:

```powershell
dotnet run --project TheParser -- interpret examples/hello.parser
```

Add `--debug` to print the lexer tokens and abstract syntax tree before execution:

```powershell
dotnet run --project TheParser -- interpret examples/arithmetic.parser --debug
```

For the command-line help:

```powershell
dotnet run --project TheParser -- help
dotnet run --project TheParser -- help interpret
```

## Language overview

Each statement ends with a semicolon. Whitespace can appear between tokens.

### Values and expressions

- Numbers are non-negative integers, such as `42`.
- Strings use double quotes. `\n` inserts a newline inside a string.
- Arithmetic supports `+`, `-`, `*`, and `/`; multiplication and division bind more tightly than addition and subtraction.
- Unary `+` and `-` are supported, as are parenthesized expressions.
- `+` also concatenates strings and printable values.

### Variables

Declare a variable with `@`, optionally supplying an initializer:

```
@total = 21 * 2;
@empty;
```

Variable names currently contain letters only.

### Built-in functions

- `Print(value)` writes one value to standard output.
- `Ask()` reads one line from standard input and returns it as a string.
- `ConvertToNumber(string)` converts a numeric string to a number.

For example:

```
@name = Ask();
Print("Hello, " + name + "!\n");
```

## Examples

The [`examples`](examples) directory contains runnable programs that demonstrate the language features:

```powershell
dotnet run --project TheParser -- interpret examples/hello.parser
dotnet run --project TheParser -- interpret examples/arithmetic.parser
dotnet run --project TheParser -- interpret examples/variables.parser
dotnet run --project TheParser -- interpret examples/input.parser
```

`input.parser` waits for a line of input. The other examples run without interaction.
