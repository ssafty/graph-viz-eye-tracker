from igraph import *
from xml.etree.ElementTree import *
from random import *
import uuid


def tree(nodes, edges, scale):
    """ creates a tree with given node count, where each node has the given edge count"""
    g = Graph.Tree(nodes, edges)
    layout = g.layout_fruchterman_reingold_3d()
    layout.scale(scale)
    return g, layout


def bipartite(nodes, prob, scale):
    g = Graph.Random_Bipartite(n1=nodes, n2=nodes, m=prob)
    layout = g.layout_fruchterman_reingold_3d()
    layout.scale(scale)
    # TODO filter edges
    return g, layout


def random_geometric(nodes, radius, scale):
    g = Graph.GRG(nodes, radius)
    layout = g.layout_fruchterman_reingold_3d()
    layout.scale(scale)
    edges = g.get_edgelist()
    print(len(edges))
    rem = sample(edges, int(len(edges) * 0.99))
    g.delete_edges(rem)
    print(len(g.get_edgelist()))
    return g, layout


def write_xml(graph, layout, prefix):
    root = Element('graphml')
    graph = Element('graph')
    root.append(graph)
    for i, p in enumerate(layout.coords):
        node = Element('node')
        node.set('count', '1')
        node.set('id', str(i))
        node.set('x', str(p[0] * uniform(0.9, 1.1)))
        node.set('y', str(p[1] * uniform(0.9, 1.1)))
        node.set('z',  str(p[2] * uniform(0.9, 1.1)))
        graph.append(node)

    for e in g.get_edgelist():
        edge = Element('edge')
        edge.set('id', str(uuid.uuid1()))
        edge.set('source', str(e[0]))
        edge.set('target', str(e[1]))
        graph.append(edge)

    tree = ElementTree(root)
    tree.write("{}.xml".format(prefix))

g, l = tree(150, 4, 0.5)
write_xml(g, l, "tree")

g, l = bipartite(100, 350, 0.8)
write_xml(g, l, "bip")

g, l = random_geometric(150, 15, 5)
write_xml(g, l, "sphere")
