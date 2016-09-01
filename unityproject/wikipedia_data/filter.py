import optparse
from random import sample
from shutil import copyfile
import time
import json
from random import random
from functools import reduce
from itertools import chain

parser = optparse.OptionParser()

parser.add_option('-s', '--sample',
                  type="int", nargs=1, action="store", dest="SampleSize",
                  help="", default="2000")

parser.add_option('-n', '--nodes',
                  type="int", nargs=1, action="store", dest="NodeCount",
                  help="", default="5")
options, args = parser.parse_args()

SAMPLE_SIZE = options.SampleSize
NODE_COUNT = options.NodeCount
WORK_FILE = copyfile("dataset.json", "filtered_dataset.json")


loadedEdges = []
loadedNodes = []


def filterNodes():
    avgCount = reduce(lambda x, y: int(x) + int(y),
                      map(lambda x: x["count"], loadedNodes)) / len(loadedNodes)



    highList = []
    midList = []
    restList = []
    for n in loadedNodes:
        count = int(n["count"])
        if len(highList) <= int(0.01 * SAMPLE_SIZE) and count >= avgCount*2:
            highList.append(n)
        elif len(midList) <= int(0.01 * SAMPLE_SIZE) and count >= avgCount and count < avgCount*2:
            midList.append(n)
        elif len(restList) <= int(0.99 * SAMPLE_SIZE):
            restList.append(n)
    return list(chain.from_iterable([highList,midList,restList]))


with open(WORK_FILE) as data_file:
    data = json.load(data_file)
    loadedEdges = data["edges"]
    loadedNodes = data["nodes"]


validNodes = filterNodes()
nodeNames = list(map(lambda x: x["id"], validNodes))
validEdges = []







def filterEdges():
    startList = {k: 0 for k in nodeNames}
    targetList = {k: 0 for k in nodeNames}
    conn = []
    for edge in loadedEdges:
        start = edge["source"] in nodeNames
        end = edge["target"] in nodeNames
        if start and end:
            if (startList[edge["source"]] <= NODE_COUNT and targetList[edge["target"]] <= NODE_COUNT) or NODE_COUNT == -0:
                validEdges.append(edge)
                startList[edge["source"]]  = startList[edge["source"]] +1
                targetList[edge["target"]] = targetList[edge["target"]] +1
start = time.time()


print("[Loaded] Nodes " + str(len(loadedNodes)))
print("[Loaded] Edges " + str(len(loadedEdges)))

filterEdges()
print("[Filtered] Nodes " + str(len(validNodes)))
print("[Filtered] Edges " + str(len(validEdges)))
print("Duration: " + str(time.time() - start))

with open(WORK_FILE, 'w') as wr:
    json.dump({"nodes": list(validNodes), "edges": validEdges}, wr)
