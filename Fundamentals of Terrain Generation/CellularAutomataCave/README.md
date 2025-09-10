# CellularAutomataCave
A simple implementation of cellular automata following these rules:\
1\. Any live cell with fewer than two live neighbours dies, as if by underpopulation.\
2\. Any live cell with two or three live neighbours lives on to the next generation.\
3\. Any live cell with more than three live neighbours dies, as if by overpopulation.\
4\. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

After cellular automata phase finished, use flood fill algorithm to remove caves smaller than prefer size.\
Then use A* path finding algorithm to connect sub caves with main cave.

## MonoGame Version
3.8.4

## Platform
Windows (Desktop GL)

## Usage
Press "N" key to generate next map.

## Reference
[https://code.tutsplus.com/generate-random-cave-levels-using-cellular-automata--gamedev-9664t](https://code.tutsplus.com/generate-random-cave-levels-using-cellular-automata--gamedev-9664t)\
[https://zh.wikipedia.org/zh-tw/Flood_fill](https://zh.wikipedia.org/zh-tw/Flood_fill)\
[https://www.kodeco.com/2425-procedural-level-generation-in-games-using-a-cellular-automaton-part-1](https://www.kodeco.com/2425-procedural-level-generation-in-games-using-a-cellular-automaton-part-1)\
[https://www.kodeco.com/2424-procedural-level-generation-in-games-using-a-cellular-automaton-part-2](https://www.kodeco.com/2424-procedural-level-generation-in-games-using-a-cellular-automaton-part-2)\
[https://www.kodeco.com/3016-introduction-to-a-pathfinding](https://www.kodeco.com/3016-introduction-to-a-pathfinding)
