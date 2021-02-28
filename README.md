# UnoWebServer
Simple web server that allows users to cross-test their UNO AI clients

# Codes of different UNO primitives

1. Colors
  * Red = 0
  * Green = 1
  * Blue = 2
  * Yellow = 3
  
2. Types
  * [Numeric](https://i.pinimg.com/originals/fc/0c/e1/fc0ce1489779e1fe7a58899327141599.png) = 0
  * [Reverse](https://i.pinimg.com/originals/0a/19/24/0a1924d437254f66c7ae563d2d0bb462.webp) = 1
  * [Skip](https://www.cuou.net/Uploads/ueditor/php/upload/image/20190515/1557911874155146.png) = 2
  * [Take two](https://www.uno-kartenspiel.de/wp-content/uploads/2019/08/zwei-ziehen-karte.jpg) = 3
  * [Take four and choose color](https://www.ultraboardgames.com/uno/gfx/wild4.jpg) = 4
  * [Choose color](https://www.cuou.net/Uploads/ueditor/php/upload/image/20190515/1557913541998387.jpg) = 5
  
# Codes of different statuses
  
1. Game status (Status from BoardResponse)
  * In Process = 0
  * Win = 1
  * Lose = 2
    
2. Match status (Status from StartMatchResponse)
  * Queued = 0
  * Matched = 1
  
 # Requests flow
 
 1. Get GUID token using match/token. Now you shall authenticate requests with it
 2. Get match id using match/start. If response contains status queued wait for a bit and try again
 3. Once you get match id you can check current board state with game/board and make a move with game/move
