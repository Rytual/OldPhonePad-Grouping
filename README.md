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

## The Rhythm of Repetition

I remember developing a muscle memory for texting back in high school. You'd tap-tap-tap in rapid succession for one letter, pause slightly, then tap-tap-tap again for the next. It was all about the rhythm - the grouping of taps separated by tiny pauses. This implementation captures that exact mental model: identify the groups, decode each group.

This decoder solves the classic old phone keypad challenge using a **grouping approach** that clusters consecutive identical digits before processing them.

## The Challenge

Decode old phone keypad input into readable text. The keypad layout:

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

## Why Grouping?

This approach treats the input as a sequence of "runs" or "groups" of identical characters. Each group represents a single output character, with the group length determining which character in the key's sequence to output.

**How it works:**
1. Scan through the input
2. Identify consecutive identical digits (a "group")
3. Count the length of each group
4. Use modulo arithmetic to map group length → character
5. Spaces break groups, allowing same-key repeats

**Pros:**
- Intuitive mental model (matches how we actually typed)
- Clean separation between grouping and decoding logic
- Easy to visualize and debug (see the groups explicitly)
- Naturally handles the pause/space behavior
- Can leverage regex for elegant grouping (optional)

**Cons:**
- Requires scanning/grouping before decoding (not true single-pass)
- Backspace handling breaks the pure grouping model
- Slightly more complex than simple state tracking
- The grouping step adds cognitive overhead

Good for when you want to match the mental model of typing or when you need to preprocess input into logical chunks. Like parsing before evaluating.

## Getting Started

### Prerequisites

- .NET 8.0 or later
- An appreciation for patterns and rhythm

### Running the Code

```bash
# Clone the repository
git clone https://github.com/yourusername/OldPhonePad-Grouping.git
cd OldPhonePad-Grouping

# Build the project
dotnet build

# Run tests
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

This project includes 45+ unit tests covering:

- All provided examples
- Edge cases (empty input, multiple backspaces, excessive spaces)
- Single character decoding for all keys
- Grouping-specific tests - consecutive digits, group separation, complex patterns
- Cycling behavior (pressing a key more times than it has letters)
- Pause handling (spaces between same-key presses)
- Backspace operations within and between groups
- Special keys (symbols on key 1, space on key 0)
- Complex real-world scenarios like SOS, HELLO WORLD, CAT
- Error handling (null input, missing send character)
- Stress tests with long inputs, long groups, repeated patterns and alternating digits

The grouping approach works well for tests involving repeated patterns and explicit group boundaries.

## Project Structure

```
OldPhonePad-Grouping/
├── src/
│   ├── OldPhonePad.cs                    # Grouping decoder implementation
│   └── OldPhonePad.Grouping.csproj
├── tests/
│   ├── OldPhonePadTests.cs              # Test suite
│   └── OldPhonePad.Grouping.Tests.csproj
├── .github/
│   └── workflows/
│       └── dotnet.yml                    # CI/CD pipeline
├── .gitignore
├── LICENSE
└── README.md
```

## Implementation Details

The grouping algorithm works in two phases:

**Phase 1: Group Extraction**
```csharp
// Extract consecutive identical digits
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

The implementation includes two variants:
- **Iterative grouping**: Manual character-by-character scanning
- **Regex grouping**: Uses pattern matching for more functional style

Special handling:
- Spaces act as group separators (reset grouping)
- Backspaces interrupt grouping and delete from output
- Different digit = new group starts

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

## Extensions & Ideas

The grouping structure makes some extensions pretty natural:

- Implement parallel group processing for very long inputs
- Add group visualization/debugging output
- Create a "group optimizer" that suggests minimal keypresses
- Build a reverse encoder with automatic space insertion
- Add regex-based group validation
- Implement group-based undo (undo last group)
- Animated typing visualization showing groups forming
- Group statistics like avg group length or most common groups

## Alternatives

Check out my other implementations:
- **OldPhonePad-DictionaryState**: Simple dictionary with manual state tracking
- **OldPhonePad-FSM**: Finite state machine with formal state transitions
- **OldPhonePad-OOP**: Object-oriented design with separate classes
- **OldPhonePad-RegexStack**: Regex preprocessing with stack-based evaluation

Each explores different ways to solve the same problem.

## Contributing

Found a bug? Have an improvement? Feel free to open an issue or submit a pull request. The grouping structure should make it easy to add new preprocessing steps or alternative grouping strategies.

Please follow standard C# conventions and include tests for any new functionality.

## License

MIT License - see LICENSE file for details. Use it, modify it, regroup it. Remember: every pause between taps was a conscious choice back in the day.

## Acknowledgments

- Iron Software for the coding challenge that inspired this grouping exploration
- Everyone who ever counted tap-tap-tap-pause-tap-tap in their head while texting
- The designers who figured out that pauses could separate same-key letters
- Nokia, for phones with such satisfying tactile keypads you could text blindfolded

---

Built with pattern recognition and a deep appreciation for chunking. Remember: the fastest T9 texters didn't think about individual letters - they thought in groups.

*Last updated: October 2025*
