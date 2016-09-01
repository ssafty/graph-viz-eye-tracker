import json
import sys
import optparse
from shutil import copyfile
import time
from itertools import chain
from multiprocessing import Pool
import pprint
from random import random

parser = optparse.OptionParser()

parser.add_option('-t', '--threads',
                  type="int", nargs=1, action="store", dest="Threads",
                  help="", default="2")

parser.add_option('-f', '--file',
                  type="string", nargs=1, action="store", dest="File",
                  help="", default="dataset.json")

parser.add_option('-i', '--iter',
                  type="int", nargs=1, action="store", dest="Iterations",
                  help="", default="1")
options, args = parser.parse_args()

THREADS = options.Threads
FILE = options.File
ITERATIONS = options.Iterations


WORK_FILE = copyfile(FILE, "full_2_" + FILE)


with open(WORK_FILE) as data_file:
    data = json.load(data_file)
    loadedEdges = data["edges"]
    loadedNodes = data["nodes"]


def thin(edges):
    newEdges = {}
    validNodes = []
    for i in edges:
        for j in edges:
            if i["target"] == j["source"]:
                for k in edges:
                    if j["target"] == k["source"]:
                        if True:
                            copy = i.copy()
                            validNodes.append(copy["target"])
                            copy["target"] = k["source"]
                            newEdges[copy["id"]] = i
                            if i != k:
                                newEdges[k["id"]] = k
                            validNodes.append(k["source"])
                            if j in edges:
                                edges.remove(j)

    newNodes = []
    for n in loadedNodes:
        if n["id"] in validNodes:
            newNodes.append(n)
    print("From " +str(len(edges)) +" to " + str(len(newEdges.values())) + " edges")
    return (newNodes, list(newEdges.values()))


def chunks(l, n):
    """Yield successive n-sized chunks from l."""
    for i in range(0, len(l), n):
        yield l[i:i + n]

ll = list(chunks(loadedEdges, len(loadedEdges) // THREADS))


def write_to_disk(nodes, edges):
    with open(WORK_FILE, 'w') as out:
        print("[Before] NODES: " + str(len(loadedNodes)), "EDGES: " + str(len(loadedEdges)))
        print("[After] NODES: " + str(len(nodes)), "EDGES: " + str(len(edges)))
        m = {"nodes": nodes, "edges": edges}
        json.dump(m, out)

for i in range(0, ITERATIONS):
    p = Pool(THREADS)
    start = time.time()
    res = p.map(thin, ll)
    final_edges = []
    final_nodes = []

    for tp in res:
        for node in tp[0]:
            if node not in final_nodes:
                final_nodes.append(node)
        for edge in tp[1]:
            if edge not in final_edges:
                final_edges.append(edge)

    write_to_disk(final_nodes, final_edges)
    loadedEdges = final_edges[:]
    loadedNodes = final_nodes[:]
    print(str(i)+" Finished: " + str(time.time() - start))
