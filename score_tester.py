import random


action = False
random_mode = True

players = [0, 1, 2, 3, 4, 5, 6, 7]
random.shuffle(players)
scores = [30, 30, 30, 30, 30, 30, 30, 30] if action else [0, 0, 0, 0, 0, 0, 0 ,0]
game_count = [0, 0, 0, 0, 0, 0, 0, 0]
played_with = [[], [], [], [], [], [], [], []]
pairs = []
important_things = []
#rand_info = [0.5, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99, 0.99]
rand_info = [0.5, 0.65, 0.7, 0.8, 0.9, 0.95, 0.97, 0.99]
for _ in range (0, 56):
    cur = min(game_count)
    f_player = game_count.index(cur)
    for i in range(f_player + 1, len(players)):
        if game_count[i] == cur and f_player not in played_with[i]:
            pairs.append((f_player, i))
            game_count[f_player] += 1
            game_count[i] += 1
            played_with[i].append(f_player)
            played_with[f_player].append(i)
            break

for pair in pairs:
    if random_mode:
        stronger = pair[0] if players[pair[0]] > players[pair[1]] else pair[1]
        weaker = pair[0] if stronger == pair[1] else pair[1]

        diff = players[stronger] - players[weaker]
        mark = rand_info[diff]
        rand = random.random()

        winner = stronger if rand < mark else weaker
    else:
        winner = pair[0] if players[pair[0]] > players[pair[1]] else pair[1]
    loser = pair[0] if winner == pair[1] else pair[1]

    if action:
        diff_scores = min(round(scores[loser] / 3), 15)
        scores[winner] += diff_scores
        scores[loser] -= diff_scores
    else:
        scores[winner] += 1

    # if players[stronger] == 7:
    #     important_things.append(
    #         "\nFight: " + str(players[winner]) + " vs " + str(players[loser]) + ". random = " + str(rand) + " then mark= " + str(mark) +
    #         "\n  So winner is " + str(players[winner]) +
    #         "\n   Winner get " + str(diff_scores) + " points")

results = []
for i in range(0, len(players)):
    results.append((players[i], scores[i]))
sorted_by_power = sorted(results, key=lambda tup: tup[0])

for i in sorted_by_power:
    print("Player with power " + str(i[0]) + " have " + str(i[1]) + " points")
for i in important_things:
    print(i)