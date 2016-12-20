__author__ = 'saftophobia'

import numpy as np
import socket
import time

IP = '127.0.0.1'
PORT = 5007

print "Welcome to UDP Server Emulator v0.1"
print "Emulating a server on " + str(IP) + ":" + str(PORT)
print "Please press Control+C when you no longer need the emulator running ... "

server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def broadcast_frequency(x, y, seconds_count, count_per_second):
    for j in range(0, seconds_count * count_per_second):
        x_noised, y_noised = str(x + np.random.normal(0, 0.008)), str(y + np.random.normal(0, 0.008))
        server.sendto("(%s,%s)" %(x_noised, y_noised), (IP, PORT))

        time.sleep(1.0/count_per_second)

while True:
    broadcast_frequency(np.random.random(), np.random.random(), seconds_count = 6, count_per_second= 30)