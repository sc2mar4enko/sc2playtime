from System.Collections.Generic import *

import sc2reader

replays = sc2reader.load_replays(replaysPath, load_level=0)

timePlayed = 0
gamesPlayed = 0

for replay in replays:
    timePlayed += replay.length.seconds
    gamesPlayed += 1

hoursPlayed = timePlayed / 60 / 60
formattedHours = f"{hoursPlayed:.1f}"

result = [gamesPlayed, formattedHours]
stringResult = list(map(str, result))


def time():
    return List[str](stringResult)