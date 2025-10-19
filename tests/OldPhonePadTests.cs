using Xunit;
using OldPhonePad.Grouping;
using System;

namespace OldPhonePad.Grouping.Tests
{
    /// <summary>
    /// Unit tests for the OldPhonePad decoder using the Grouping approach.
    /// These tests verify the grouping algorithm correctly identifies and processes
    /// consecutive identical digits.
    /// </summary>
    public class OldPhonePadDecoderTests
    {
        #region Basic Functionality Tests

        [Fact]
        public void DecodesSimpleEJustFine()
        {
            // Just like typing 'E' on a Nokia 3310
            var result = OldPhonePadDecoder.OldPhonePad("33#");
            Assert.Equal("E", result);
        }

        [Fact]
        public void DecodesBWithBackspace()
        {
            // Type "CA" then backspace the A, leaving just B
            var result = OldPhonePadDecoder.OldPhonePad("227*#");
            Assert.Equal("B", result);
        }

        [Fact]
        public void DecodesHelloNoProblem()
        {
            // The classic HELLO - brings back memories of SMS on flip phones
            var result = OldPhonePadDecoder.OldPhonePad("4433555 555666#");
            Assert.Equal("HELLO", result);
        }

        [Fact]
        public void DecodesTuringCorrectly()
        {
            // The mystery example: "8 88777444666*664#"
            // Let's decode: 8=T, space, 88=U, 777=R, 444=I, 666=O (but then *664)
            // Backspace removes O, then 66=N, 4=G
            // Result: TURING
            var result = OldPhonePadDecoder.OldPhonePad("8 88777444666*664#");
            Assert.Equal("TURING", result);
        }

        #endregion

        #region Edge Cases - Empty and Minimal Input

        [Fact]
        public void ShouldHandleJustSend()
        {
            // Nothing typed, just hit send
            var result = OldPhonePadDecoder.OldPhonePad("#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleSpaceBeforeSend()
        {
            // Space then send - should be empty
            var result = OldPhonePadDecoder.OldPhonePad(" #");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleBackspaceOnEmpty()
        {
            // Try to backspace when there's nothing there
            var result = OldPhonePadDecoder.OldPhonePad("*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldHandleMultipleBackspacesOnEmpty()
        {
            // Mashing backspace like a maniac
            var result = OldPhonePadDecoder.OldPhonePad("***#");
            Assert.Equal("", result);
        }

        #endregion

        #region Single Character Tests

        [Fact]
        public void ShouldDecodeFirstLetterOfEachKey()
        {
            Assert.Equal("A", OldPhonePadDecoder.OldPhonePad("2#"));
            Assert.Equal("D", OldPhonePadDecoder.OldPhonePad("3#"));
            Assert.Equal("G", OldPhonePadDecoder.OldPhonePad("4#"));
            Assert.Equal("J", OldPhonePadDecoder.OldPhonePad("5#"));
            Assert.Equal("M", OldPhonePadDecoder.OldPhonePad("6#"));
            Assert.Equal("P", OldPhonePadDecoder.OldPhonePad("7#"));
            Assert.Equal("T", OldPhonePadDecoder.OldPhonePad("8#"));
            Assert.Equal("W", OldPhonePadDecoder.OldPhonePad("9#"));
        }

        [Fact]
        public void ShouldDecodeLastLetterOfEachKey()
        {
            Assert.Equal("C", OldPhonePadDecoder.OldPhonePad("222#"));
            Assert.Equal("F", OldPhonePadDecoder.OldPhonePad("333#"));
            Assert.Equal("I", OldPhonePadDecoder.OldPhonePad("444#"));
            Assert.Equal("L", OldPhonePadDecoder.OldPhonePad("555#"));
            Assert.Equal("O", OldPhonePadDecoder.OldPhonePad("666#"));
            Assert.Equal("S", OldPhonePadDecoder.OldPhonePad("7777#"));
            Assert.Equal("V", OldPhonePadDecoder.OldPhonePad("888#"));
            Assert.Equal("Z", OldPhonePadDecoder.OldPhonePad("9999#"));
        }

        #endregion

        #region Grouping-Specific Tests

        [Fact]
        public void ShouldGroupConsecutiveDigits()
        {
            // "222" should be treated as one group of 3
            var result = OldPhonePadDecoder.OldPhonePad("222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void ShouldSeparateGroupsAtDifferentDigits()
        {
            // "223" should be two groups: "22" and "3"
            var result = OldPhonePadDecoder.OldPhonePad("223#");
            Assert.Equal("BD", result);
        }

        [Fact]
        public void ShouldSeparateGroupsAtSpaces()
        {
            // "22 22" should be two separate groups of "22"
            var result = OldPhonePadDecoder.OldPhonePad("22 22#");
            Assert.Equal("BB", result);
        }

        [Fact]
        public void ShouldHandleComplexGrouping()
        {
            // "222 2 22" has three groups
            var result = OldPhonePadDecoder.OldPhonePad("222 2 22#");
            Assert.Equal("CAB", result);
        }

        [Fact]
        public void ShouldHandleSingleDigitGroups()
        {
            // Each digit is its own group
            var result = OldPhonePadDecoder.OldPhonePad("2 3 4 5#");
            Assert.Equal("ADGJ", result);
        }

        #endregion

        #region Cycling Tests

        [Fact]
        public void ShouldCycleWhenPressingTooMany()
        {
            // Press 2 six times - should cycle back to C
            var result = OldPhonePadDecoder.OldPhonePad("222222#");
            Assert.Equal("C", result);
        }

        [Fact]
        public void ShouldCycleOn7Key()
        {
            // Key 7 has 4 letters: P, Q, R, S
            // 77777 = 5 presses = P (cycles once, lands on P again)
            var result = OldPhonePadDecoder.OldPhonePad("77777#");
            Assert.Equal("P", result);
        }

        [Fact]
        public void ShouldCycleOn9Key()
        {
            // Key 9 has 4 letters: W, X, Y, Z
            // 99999 = 5 presses = W (cycles once)
            var result = OldPhonePadDecoder.OldPhonePad("99999#");
            Assert.Equal("W", result);
        }

        #endregion

        #region Space (Pause) Tests

        [Fact]
        public void ShouldHandleSpaceBetweenSameKeys()
        {
            // "222 2 22" should give CAB (from the problem description)
            var result = OldPhonePadDecoder.OldPhonePad("222 2 22#");
            Assert.Equal("CAB", result);
        }

        [Fact]
        public void ShouldHandleMultipleSpaces()
        {
            // Multiple spaces should work the same as one
            var result = OldPhonePadDecoder.OldPhonePad("2   2#");
            Assert.Equal("AA", result);
        }

        [Fact]
        public void ShouldHandleLeadingSpaces()
        {
            // Spaces before any input
            var result = OldPhonePadDecoder.OldPhonePad("  2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void ShouldHandleTrailingSpaces()
        {
            // Spaces after input but before send
            var result = OldPhonePadDecoder.OldPhonePad("2  #");
            Assert.Equal("A", result);
        }

        #endregion

        #region Backspace Tests

        [Fact]
        public void ShouldBackspaceLastCharacter()
        {
            // Type C then backspace
            var result = OldPhonePadDecoder.OldPhonePad("222*#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldBackspaceMiddleOfWord()
        {
            // Type CD then backspace, then add D again
            // 222=C, 3=D, backspace removes D, 3=D again
            var result = OldPhonePadDecoder.OldPhonePad("2223*3#");
            Assert.Equal("CD", result);
        }

        [Fact]
        public void ShouldHandleMultipleBackspaces()
        {
            // Type CC then backspace twice
            var result = OldPhonePadDecoder.OldPhonePad("222333**#");
            Assert.Equal("", result);
        }

        [Fact]
        public void ShouldBackspaceAndContinue()
        {
            // Type CD, backspace D, then add O
            // 222=C, 3=D, backspace removes D, 666=O
            var result = OldPhonePadDecoder.OldPhonePad("2223*666#");
            Assert.Equal("CO", result);
        }

        [Fact]
        public void ShouldHandleBackspaceWithinGroup()
        {
            // Backspace should break the group
            var result = OldPhonePadDecoder.OldPhonePad("22*2#");
            Assert.Equal("A", result);
        }

        #endregion

        #region Special Keys Tests

        [Fact]
        public void ShouldHandleKey1Symbols()
        {
            // Key 1 has symbols: &'(
            var result = OldPhonePadDecoder.OldPhonePad("1#");
            Assert.Equal("&", result);
        }

        [Fact]
        public void ShouldHandleKey1AllSymbols()
        {
            Assert.Equal("&", OldPhonePadDecoder.OldPhonePad("1#"));
            Assert.Equal("'", OldPhonePadDecoder.OldPhonePad("11#"));
            Assert.Equal("(", OldPhonePadDecoder.OldPhonePad("111#"));
        }

        [Fact]
        public void ShouldHandleKey0AsSpace()
        {
            // Key 0 is space
            var result = OldPhonePadDecoder.OldPhonePad("220#");
            Assert.Equal("B ", result);
        }

        [Fact]
        public void ShouldHandleMultipleKey0()
        {
            // Multiple presses of 0 cycle through (but there's only 1 char)
            // 2=A, 000 is one group of 3 zeros -> one space (cycling)
            var result = OldPhonePadDecoder.OldPhonePad("2000#");
            Assert.Equal("A ", result);
        }

        #endregion

        #region Complex Real-World Scenarios

        [Fact]
        public void ShouldDecodeHI()
        {
            // HI: 44=H, space, 444=I
            var result = OldPhonePadDecoder.OldPhonePad("44 444#");
            Assert.Equal("HI", result);
        }

        [Fact]
        public void ShouldDecodeYES()
        {
            // YES: 999=Y, 33=E, 7777=S
            var result = OldPhonePadDecoder.OldPhonePad("999337777#");
            Assert.Equal("YES", result);
        }

        [Fact]
        public void ShouldDecodeNO()
        {
            // NO: 66 66 -> need space between same keys
            var result = OldPhonePadDecoder.OldPhonePad("66 666#");
            Assert.Equal("NO", result);
        }

        [Fact]
        public void ShouldDecodeSOS()
        {
            // SOS: 7777=S, 666=O, 7777=S (different keys so no space needed for first->second)
            var result = OldPhonePadDecoder.OldPhonePad("7777666 7777#");
            Assert.Equal("SOS", result);
        }

        [Fact]
        public void ShouldDecodeHELLO_WORLD()
        {
            // HELLO YORLD with space character between words
            // 44=H, 33=E, 555=L, space, 555=L, 666=O, 0=space, space, 999=Y, 666=O, 777=R, 555=L, 3=D
            var result = OldPhonePadDecoder.OldPhonePad("4433555 5556660 9996667775553#");
            Assert.Equal("HELLO YORLD", result);
        }

        [Fact]
        public void ShouldDecodeCAT()
        {
            // CAT demonstrates grouping across different keys
            var result = OldPhonePadDecoder.OldPhonePad("2228#");
            Assert.Equal("CT", result);
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public void ShouldThrowOnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() =>
                OldPhonePadDecoder.OldPhonePad(null!));
        }

        [Fact]
        public void ShouldThrowOnMissingSend()
        {
            Assert.Throws<ArgumentException>(() =>
                OldPhonePadDecoder.OldPhonePad("222"));
        }

        #endregion

        #region Stress Tests

        [Fact]
        public void ShouldHandleLongInput()
        {
            // A really long message - like texting an essay in 2003
            // 44=H, 33=E, 555=L, space, 555=L, 666=O, space, 999=Y, 666=O, 777=R, 555=L, 3=D
            var result = OldPhonePadDecoder.OldPhonePad("4433555 555666 9996667775553#");
            Assert.Equal("HELLOYORLD", result);
        }

        [Fact]
        public void ShouldHandleManyBackspaces()
        {
            // Type something, then backspace most of it
            var result = OldPhonePadDecoder.OldPhonePad("22233344455566677788899***************2#");
            Assert.Equal("A", result);
        }

        [Fact]
        public void ShouldHandleLongGroups()
        {
            // Very long groups should cycle correctly
            var result = OldPhonePadDecoder.OldPhonePad("22222222222222222222#");
            Assert.Equal("B", result); // 20 presses = 20 % 3 = 2 -> B
        }

        [Fact]
        public void ShouldHandleAlternatingDigits()
        {
            // Tests grouping with frequent group changes
            var result = OldPhonePadDecoder.OldPhonePad("23456789#");
            Assert.Equal("ADGJMPTW", result);
        }

        [Fact]
        public void ShouldHandleRepeatedGrouping()
        {
            // Same groups repeated
            var result = OldPhonePadDecoder.OldPhonePad("22 22 22 22#");
            Assert.Equal("BBBB", result);
        }

        [Fact]
        public void GroupingWithMixedLengths()
        {
            // Groups of different lengths - tests grouping logic
            // 2=A, space, 333=F, space, 55555=K (5%3=2, so 2nd letter of 5 key)
            var result = OldPhonePadDecoder.OldPhonePad("2 333 55555#");
            Assert.Equal("AFK", result);
        }

        [Fact]
        public void BackspaceBreaksGroup()
        {
            // Backspace should interrupt grouping
            // 222 is a group (C), then backspace deletes it, then 2 is new group (A)
            var result = OldPhonePadDecoder.OldPhonePad("222*2#");
            Assert.Equal("A", result);
        }

        #endregion
    }
}
