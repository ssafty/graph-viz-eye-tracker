__author__ = 'saftophobia'

import os, inspect, math
from collections import Counter
from random import random
import optparse

"""
This script transforms the raw data from its standard format to JSON.
Hence, JSON to XML libraries can be used.
More features are implemented for the project such as the number of edges per node.
"""

parser = optparse.OptionParser()

parser.add_option('-s', '--sample',
    type="float", nargs=1, action="store", dest="SamplingValue",
    help="sampling value between 0 and 1", default="0.20")
options, args = parser.parse_args()

SAMPLING_VALUE = options.SamplingValue #default 0.2

def print_progress(w, list_size):
    partitions = list_size/10
    if (w % partitions == 0 and w > 0 and w <= list_size):
        progress = int((w / float(list_size)) * 100 )
        print "[PROGRESS] " + "." * progress + " " * (101-progress) + "{0:.0f}%".format(w/float(list_size) * 100)

EDGES = "data/links.tsv"
OUTPUT_XML = "dataset.json"

words = [] #list of all words in the links document
edges = [] #list of JSON edges
with open(EDGES, 'r') as nodes_file:
    nodes_list = nodes_file.read().splitlines()[12:] #remove first 12 commented lines
    sampled_list = [node for node in nodes_list if random() <= SAMPLING_VALUE]
    for w, node in enumerate(sampled_list):
        kv_pair = node.split("\t")
        words.extend(kv_pair) #split words to list to get Frequency
        edges.append("{ \"source\": \"" + kv_pair[0] + "\",\"target\": \"" + kv_pair[1] + "\",\"id\": \"link_" + str(w) + "\"}")
        print_progress(w, len(sampled_list))

words_count = Counter(words)
print "[DONE] Top three words are :" + str(words_count.most_common(3))

nodes = [] #list of JSON nodes
with open(OUTPUT_XML, 'w') as output_file:
    print "[NOTE] Currently truncating " + os.path.dirname(os.path.abspath(inspect.getfile(inspect.currentframe()))) + \
          "/" + OUTPUT_XML + " ..."
    output_file.truncate()

    print "[NOTE] Writing nodes ..."
    output_file.write("{ \"nodes\": [")
    for key,value in words_count.iteritems():
        nodes.append("{ \"id\": \"" + key + "\", \"count\": \""+ str(value) +"\" }")
    output_file.write(",".join(nodes))
    print "[NOTE] Nodes: " + str(len(nodes))
    output_file.write("], \"edges\": [")

    print "[NOTE] Writing edges ..."
    output_file.write(",".join(edges))
    output_file.write("] }")
    print "[NOTE] Edges: " + str(len(edges))
    print "[NOTE] Finished!"


print "[TODO] install https://github.com/godlikemouse/ForceDirectedLayout"
print "[TODO] change all occurrances of \"(filename_)\" to \"(filename_.c_str())\" if gcc is old"
print "[TODO] add #include <string.h> if strcmp is not identified"
print "[TODO] run ./fdl --in=dataset.json --out=output.xml --out-type=GraphML --mode=3D --in-type=JSON"
