# Hardy-Ramanujan Number

This is a program built to find  Hardy-Ramanujan numbers. It uses exaustive search to find the numbers; it tires every combination within a range of numbers. Hardy-Ramanujan Numbers quickly becombes very large. Therefore the software is built around partitioning the data set into smaller rangers.
Hardy-Ramanujan Numbers larger than Ta(4) becomes impractically large. To to find these numbers within a reasonable amount of time might not be feasable. Therefore this program is more to show of some techniques in exaustive search and to challenge mathematical thinking.

## Speed Results

|| Taxicab number n || Time spent (ms) || Starting from number || Ending at number ||
| 1                  | 33               | 1                     | 1                 |
| 2                  | 42               | 1                     | 12                |
| 3                  | 155              | 167                   | 437               |
| 4                  | 2180727          | 2420                  | 19499             |

These results were produced by a 2.6 GHz 6-core Intel Core i7.

## Techniques

As mentioned this program finds the taxicab numbers by exaustive search. That means every combination of the taxicab equation within a range of numbers is tried out.

We recall the taxi cab number **Ta(n)** is the smallest number that applies for.

* There are n cobinations of **x** and **y** such that **Ta(n) = x^3 + y^3**.
* **x^3 != y^3**

### Lookup Tables

For repetitive search for **Ta(n)** different values for x and y will be combined in various combinations. To speed up the calculations all cubes (x^3) within a range [n, m> are calculated once, and the sum of the cube is lookup up multiple times after it. This optimization is fairly obvious. However many cubes the computer hold before the program runs out of memory? It turns out that C# has an upper limit of 2^64 cubes in an array. This is the number of qubes the program can hold at any given time. The exaustive search must be split up into smaller subset of no more than 2^64 values.

## Exploiting Commutivity

The taxicab numbers are the sum of **x^3** and **y^3**. Due to pluss being commutative we know that **x^3 + y^3 == y^3 + x^3**. This follows by the definition of commutivity.
With this knowlege in mind we don't need to calculate **y^3 + x^3** once we know **x^3 + y^3**. This narrows down our search space below an upper bound x < N and y < N in half.


To visualize this commutivity we set up the formula **x^3 + y^3** as an **x** by **y** matrix where every cell is the result of **x^3 + y^3**.

```text
 0  1  8 27 →
 1  2  9 28
 8  9 16 35
27 28 28 54
 ↓          ↘
```

This matrix has an infinate size x ∈ [1, ∞] and y ∈ [1, ∞].

With the commutivity in mind can search the matrix x ∈ [1, ∞] and y ∈ [x, ∞].

```text
 0  1  8 27 →
 -  2  9 28
 -  - 16 35
 -  -  - 54
 ↓          ↘
```

The relevant values are listed in the figure above.

### Diving the Search Space

To get a good understanding of how **x** and **y** relate to **Ta(n)**.

From work by mathmaticians certain values of **Ta(n)** are known. For values we don't know we have upper bounds of **Ta(n)** up to **Ta(11)**. That is ingredible work.
Unfortunatly if **Ta(n)** is unknown is no simple relation between **T(n)**, **x** and **y**. In principle **x** and **y** can take any value greater han 0; [1, ∞].

To Narrow the search we set up our terations to follow the "up and right" diagonal (AKA UR-diagonal).

```text
 0  1  3  5 →
 -  2  4  7
 -  -  6  8
 -  -  -  9
 ↓          ↘
```

We visualize the search by their iteration index below.

We iterate along the search space and store all the values **x^3 + y^3** we see as a potensial **Ta(n)**. This becomes an practical limit for memory usage. This naive approach require **x * y / 2** values and **O(n)** products.

The smallest values when searching along the UR-idagonal occurs along **x == y**. Whenever we pass a the diagonal in the matrix **x == y** will reach a local minimum. this is best explored by visualising the matrix. We exploit this local minimum by discarding values lower than **x^3 + y^3** as we move along the UR-diagonal.

The way we exploit the search space is to look at the potensial values for x, y, and **x^3 + y^3**.

For **x = y** then **x** will take it's lowest potential value in **x^3 + y^3**.

```math
x^3 + y^3 = Potensial Ta(n)
y = x
Then
x^3 + x^3 = Potensial Ta(n)
2x^3 = (qube root(Potensial Ta(n))) / 2
```

For **y = 1** then **x** will take it's highest potential value in **x^3 + y^3**.

```math
x^3 + y^3 = Potensial Ta(n)
y = 1
Then
x^3 + 1 = Potensial Ta(n)
x = qube root(Potensial Ta(n)) - 1
```

From this knowlege we can test **Potensial Ta(n)** in the range **[qube root(Potensial Ta(n)) / 2, qube root(Potensial Ta(n)) - 1]**. This is a great improvement in terms of memory.

With this search technique and new memory bounds we can split the search for Ta(n) into **[1, Potensial Ta(n)]** many small searches with range **[qube root(Potensial Ta(n)) / 2, qube root(Potensial Ta(n)) - 1]**.

## Future improvementss

The overlap between serch in these subset is quite obvious. We can store the result of these searhes in one common data set. At the time of writing the next unknown taxicab number is **Ta(7)**. It is proven to have an upper bound of **Ta(7) <= 2488518931788589897523598854**.
This software has a capability to store 2^64 values. That leaves us with 2488518931788589897523598854 / 2^64 ~= 134902882 potential searches to perform. These can be spread out across multiple computers or processes to make the search go faster.