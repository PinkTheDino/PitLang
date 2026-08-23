# This is PIT 
PIT is an interpreted language made in c#. It has support for mathematical and conditional expressions and print and variable statements at the moment. 
It was based on 'Lox' from the book "Crafting Interpreters" by Robert Nystrom which was kinda interesting to me.
# Language syntax
PIT has 4 primitives: booleans, numbers (doubles), strings, and nil. 
You can set a variable by stating: ```var x = 1.0;```
You can print by stating: ```print(x);```
Statements are followed by a semicolon.
You can do basic conditional expressions like ```print( 2 * 3 < 7)``` (returns true)
Thats basically it. As im writing this I forgot about divisions by zero so oops.

# Setup (i.e. command line integration with paths) 
Add the directory of the pitlang.exe to system paths.
For this example, create a file called "myScript.pit" (extension doesn't matter as long as the file is text)
Input this text;
```
var x = true;
print(x);
```

Open a terminal instance in a folder.

<img width="669" height="581" alt="image" src="https://github.com/user-attachments/assets/256337ed-27ab-4407-b183-b497db2431a7" />

First run 
```powershell
pitlang -h
```
which should return:
<img width="1109" height="638" alt="image" src="https://github.com/user-attachments/assets/96dcc556-8395-4011-bec4-4241bd64ad45" />
Then run
```powershell
pitlang myScript.pit
```
<img width="1109" height="621" alt="image" src="https://github.com/user-attachments/assets/9c7697a9-c706-41f4-b1d9-07d68b5e39f5" />
There we go its all working.
You can also run it in debug mode by running pitlang myScript.pit -d for tokenizing and parsing tree info. 

