# This attempts to extract the uw1_object_settings.txt from Hank Morgan's repo into json, which can be easily loaded into
# c# back.
from collections import namedtuple
import json

lines = open('uw1_object_settings.txt', 'r').readlines()
entry_namedtuple = namedtuple('entry', ['itemID', 'unk1', 'unk2', 'unk3', 'unk4', 'name', 'unk5', 'unk6', 'unk7', 'unk8', 'unk9', 'unk10'])
output = []
for line in lines:
    entry = entry_namedtuple(*line.strip().split('\t'))
    output.append(entry._asdict())
    
with open('objects.json', 'w') as fhand:
    json.dump(output, fhand)