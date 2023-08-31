from System.Collections.Generic import *
import sc2reader
import gc

# Убеждаемся, что предыдущие реплеи освобождены
if 'replays' in globals():
    replays = None
    gc.collect()

replays = sc2reader.load_replays(replaysPath, load_level=0)

timePlayed = 0
gamesPlayed = 0

for replay in replays:
    timePlayed += replay.length.seconds
    gamesPlayed += 1

hoursPlayed = timePlayed / 3600  # 60 * 60
formattedHours = f"{hoursPlayed:.1f}"

result = [gamesPlayed, formattedHours]
stringResult = list(map(str, result))

# Освобождаем память явным удалением объектов
del replays
gc.collect()

def time():
    return list(stringResult)
