# Hardy-Ramanujan Number

This is a program built to find  Hardy-Ramanujan numbers. It uses exaustive search to find the numbers; it tires every combination within a range of numbers. Hardy-Ramanujan Numbers quickly becombes very large. Therefore the software is built around partitioning the data set into smaller rangers.
Hardy-Ramanujan Numbers larger than Ta(4) becomes impractically large. To to find these numbers within a reasonable amount of time might not be feasable. Therefore this program is more to show of some techniques in exaustive search and to challenge mathematical thinking.

## Speed Results

| Taxicab number n   | Time spent (ms)  | Starting from number  | Ending at number  |
|--------------------|------------------|-----------------------|-------------------|
| 1                  | 122              | 1                     | 1                 |
| 2                  | 177              | 1                     | 12                |
| 3                  | 257              | 167                   | 437               |
| 4                  | 736842           | 2420                  | 19499             |
| 5                  | -                | 19095                 | 365903            |
| 6                  | -                | -                     | -                 |
| 7                  | -                | -                     | -                 |

These results were produced by a 2.6 GHz 6-core Intel Core i7.

## Techniques

As mentioned this program finds the taxicab numbers by exaustive search. That means every combination of the taxicab equation within a range of numbers is tried out.

We recall the taxi cab number **Ta(n)** is the smallest number that applies for.

* There are n cobinations of **x** and **y** such that **Ta(n) = x^3 + y^3**.
* **x^3 != y^3**

### Lookup Tables

For repetitive search for **Ta(n)** different values for x and y will be combined in various combinations. To speed up the calculations all cubes (x^3) within a range [n, m> are calculated once, and the sum of the cube is lookup up multiple times after it. This optimization is fairly obvious. However many cubes the computer hold before the program runs out of memory? It turns out that C# has an upper limit of 2^64 cubes in an array. This is the number of qubes the program can hold at any given time. The exhaustive search must be split up into smaller subset of no more than 2^64 values.

## Exploiting Commutivity

The taxicab numbers are the sum of **x^3** and **y^3**. Due to pluss being commutative we know that **x^3 + y^3 == y^3 + x^3**. This follows by the definition of commutivity.
With this knowlege in mind we don't need to calculate **y^3 + x^3** once we know **x^3 + y^3**. This narrows down our search space below an upper bound x < N and y < N in half.

To visualize this commutivity we set up the formula **x^3 + y^3** as an **x** by **y** matrix where every cell is the result of **x^3 + y^3**.

<a id="figure-1">figure 1</a>
```text
 0  1  8 27 →
 1  2  9 28
 8  9 16 35
27 28 28 54
 ↓          ↘
```

This matrix has an infinate size x ∈ [1, ∞] and y ∈ [1, ∞].

With the commutivity in mind can search the matrix x ∈ [1, ∞] and y ∈ [x, ∞].

<a id="figure-2">figure 2</a>
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

<a id="figure-3">figure 3</a>

```text
 0  1  3  5 →
 -  2  4  7
 -  -  6  8
 -  -  -  9
 ↓          ↘
```

We visualize the search by their iteration index above. The sharp reader will discover that we are exploring the search space along the diagonal as shown in [figure 4](#figure-4). The direction is marked with arrows.

<a id="figure-4">figure 4</a>

```text
 ↗  ↗  ↗  ↗
 -  ↗  ↗  ↗
 -  -  ↗  ↗
 -  -  -  ↗
```

We iterate along the search space and store all the values **x^3 + y^3** we see as a potential **Ta(n)**. This becomes an practical limit for memory usage of the program. This naive approach require **x * y / 2** products. Memory usage grows with **O(n)**.

The smallest values when searching along the UR-diagonal occurs along **x == y**. Whenever we pass a the diagonal in the matrix **x == y** will reach a local minimum. This is best explored by visualising the matrix as shown in [figure 1](#figure-1). We exploit this local minimum by discarding values lower than **x^3 + y^3** as we move along the UR-diagonal.

The way we exploit the search space is to look at the potential values for x, y, and **x^3 + y^3**.

For **x = y** then **x** will take it's lowest potential value in **x^3 + y^3**.

```math
x^3 + y^3 = Potential Ta(n)
y = x
```

Then

```math
x^3 + x^3 = Potential Ta(n)
2x^3 = (qube root(Potential Ta(n))) / 2
```

For **y = 1** then **x** will take it's highest potential value in **x^3 + y^3**.

```math
x^3 + y^3 = Potential Ta(n)
y = 1
```
Then
```math
x^3 + 1 = Potential Ta(n)
x = qube root(Potential Ta(n)) - 1
```

From this knowledge of the lower an upper bounds for any number **Potential Ta(n)** we can test values in the range **x ∈ [qube root(Potential Ta(n)) / 2, qube root(Potential Ta(n)) - 1], y ∈ [qube root(Potential Ta(n)) / 2, qube root(Potential Ta(n)) - 1]** independant of other potensial values **Potential Ta(n)**.

With this search technique and new memory bounds we can split the search for **Ta(n)** into **[1, Potential Ta(n)]** searches.

At the time of writing we know that **Ta(6) = 24153319581254312065344** and the next unknown taxicab number is **Ta(7)**. It is proven to have an upper bound of **Ta(7) <= 2488518931788589897523598854**.
We are assumining that **Ta(7)** must be in the range of **Ta(6) + 1** and the upper bound 2488518931788589897523598854. Thiss leave us with **Ta(7) ∈ [24153319581254312065345, 2488518931788589897523598854]**.


## Future improvements

I have shown that the search for Ta(n) can be split into smaller serches within a subset.

This software is already using multiple threads within the program to make the computation faster. We can also be spread out across multiple computers to speed up the search.
