# OldPhonePad - Grouping Implementation

```
 ___________________
|  .-----------. |
| |  2  2  2   | |
| |  [GROUP]   | |
| |   = "C"    | |
| '-----------' |
| [2][3][4]     |
| [5][6][7]     |
| [8][9][0]     |
| [*][#] T9     |
'-------------------'
```

## The Challenge

This approach tackles the keypad problem by grouping consecutive identical digits before decoding. The idea is to identify "runs" of the same key press and treat each run as a single character.

The keypad layout:

```
1: &'(        2: abc       3: def
4: ghi        5: jkl       6: mno
7: pqrs       8: tuv       9: wxyz
*: backspace  0: space     #: send
```

### Examples

- `33#` → `E`
- `227*#` → `B` (type CA, backspace)
- `4433555 555666#` → `HELLO`
- `8 88777444666*664#` → `TURING`

## My Approach

I went with a grouping strategy - scan through the input, identify consecutive identical digits, count them, and decode. The grouping part felt natural since that's how you actually type on these keypads.

**How it works:**
1. Scan through input character by character
2. When you hit a digit, collect all consecutive identical digits
3. Count the group length
4. Use modulo to map length to character
5. Spaces break groups, allowing same-key repeats

**What works well:**
- Intuitive - matches how you actually think about typing
- Clean separation between grouping and decoding
- Easy to visualize and debug
- Naturally handles spaces as group separators

**What's a bit clunky:**
- Not a true single-pass algorithm (need to identify groups first)
- Backspace breaks the pure grouping model
- Slightly more complex than simple state tracking

Works okay when you want to match the mental model of typing or need to preprocess input into chunks. Check out the DictionaryState version if you want something simpler.

## Getting Started

### Prerequisites

- .NET 8.0 or later

### Running the Code

```bash
# Clone the repository
git clone https://github.com/yourusername/OldPhonePad-Grouping.git
cd OldPhonePad-Grouping

# Build and test
dotnet build
dotnet test

# For verbose test output
dotnet test --logger "console;verbosity=detailed"
```

### Using the Decoder

```csharp
using OldPhonePad.Grouping;

string result = OldPhonePadDecoder.OldPhonePad("4433555 555666#");
Console.WriteLine(result); // Output: HELLO
```

## Test Coverage

The project has 45+ tests covering:
- All provided examples
- Edge cases (empty input, backspaces, spaces)
- Single character decoding for all keys
- Grouping-specific tests (consecutive digits, group separation, patterns)
- Cycling behavior
- Pause handling
- Backspace operations within and between groups
- Special keys
- Complex scenarios like SOS, HELLO WORLD, CAT
- Error handling
- Stress tests with long groups, repeated patterns, alternating digits

The grouping approach makes it easy to test pattern-based scenarios.

## Implementation Details

The grouping algorithm works in phases:

**Phase 1: Group Extraction**
```csharp
// Identify consecutive identical digits
"222 2 22" → groups: ["222", "2", "22"]
```

**Phase 2: Group Decoding**
```csharp
// Decode each group
"222" → 3 presses of key 2 → 'C'
"2"   → 1 press of key 2 → 'A'
"22"  → 2 presses of key 2 → 'B'
Result: "CAB"
```

Special handling:
- Spaces act as group separators
- Backspaces interrupt grouping and delete from output
- Different digit starts a new group

## Visual Example

```
Input: "4433555 555666#"

Grouping phase:
[44] [33] [555] [space] [555] [666]

Decoding phase:
44   → key 4, 2 presses → 'H'
33   → key 3, 2 presses → 'E'
555  → key 5, 3 presses → 'L'
555  → key 5, 3 presses → 'L'
666  → key 6, 3 presses → 'O'

Result: "HELLO"
```

## Project Structure

```
OldPhonePad-Grouping/
├── src/
│   ├── OldPhonePad.cs                    # Grouping decoder
│   └── OldPhonePad.Grouping.csproj
├── tests/
│   ├── OldPhonePadTests.cs              # Test suite
│   └── OldPhonePad.Grouping.Tests.csproj
├── .github/
│   └── workflows/
│       └── dotnet.yml                    # CI/CD
├── .gitignore
├── LICENSE
└── README.md
```

## Other Implementations

Check out the other approaches:
- **OldPhonePad-DictionaryState**: Simple dictionary with manual state tracking
- **OldPhonePad-FSM**: Finite state machine with formal state transitions
- **OldPhonePad-OOP**: Object-oriented design with separate classes
- **OldPhonePad-RegexStack**: Regex preprocessing with stack-based evaluation

Each has different tradeoffs.

## Fun Note

The grouping logic took a bit to get right - figuring out when to break groups and how to handle backspaces within groups. At first I tried regex for the grouping part, but manual scanning ended up being clearer. The visual aspect helps a lot when debugging - you can literally see the groups forming.

## License

MIT License - see LICENSE file for details.

---

*Built for the Iron Software coding challenge - October 2025*
