import random


players = ['Gallyam', 'Nikita', 'Kirill', 'PoliMax', 'PoliMisha', 'VanyaLesha', 'Valera', 'easyBot']
random.shuffle(players)
game_count = [0, 0, 0, 0, 0, 0, 0, 0]
played_with = [[], [], [], [], [], [], [], []]
pairs = []
waves = [[(0, 2), (4, 6), (1, 3), (5, 7)],
         [(0, 4), (2, 6), (1, 5), (3, 7)],
         [(0, 6), (2, 4), (1, 7), (3, 5)],
         [(2, 1), (3, 0), (4, 7), (5, 6)],
         [(0, 7), (1, 6), (2, 5), (3, 4)],
         [(1, 0), (2, 7), (3, 6), (4, 5)],
         [(3, 2), (4, 1), (5, 0), (6, 7)]]
waveCount = 1
for wave in waves:
    print(str(waveCount) + ' wave:')
    waveCount += 1
    for game in wave:
        print('\t ' + players[game[0]] + ' vs ' + players[game[1]])
input()
