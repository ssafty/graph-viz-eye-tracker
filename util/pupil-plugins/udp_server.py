from plugin import Plugin
import socket
import os
from pyglui import ui
import traceback
import math
from operator import itemgetter

# This file has to be copied to pupil/capture_settings/plugins/

class Udp_Server(Plugin):
    """
    Udp Server broadcasts the gaze position
    over the network
    """

    def __init__(self, g_pool):
        super(Udp_Server, self).__init__(g_pool)
        self.order = .9
        self.ip = '127.0.0.1' #change to unity IP
        self.port = 5007        #change to unity port
        self.s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def init_gui(self):
        if self.g_pool.app == 'capture':
            self.menu = ui.Growing_Menu("UDP Broadcast Server")
            self.g_pool.sidebar.insert(1, self.menu)

        self.menu.append(ui.Button('Close', self.close))
        help_str = "UDP Message server"
        self.menu.append(ui.Info_Text(help_str))
    #self.menu.append(ui.Text_Input('address', self, setter=self.set_server, label='Address'))


    def deinit_gui(self):
        if self.menu and self.g_pool.app == 'capture':
            self.g_pool.sidebar.remove(self.menu)
            self.cleanup()
        self.menu = None

    def close(self): self.alive = False


    def get_init_dict(self): return {'address': self.address}


    def cleanup(self):
        """gets called when the plugin get terminated.
        This happens either voluntarily or forced.
        """
        self.deinit_gui()
        self.s.close() #close socket


    def update(self,frame,events):
        # pf = open('pupil_pos', 'a')
        # for p in events.get('pupil_positions',[]):
        #   try:
        #       pf.write(p + '\n')
        #   except:
        #       print "no pupil position"
        # msg = "Pupil\n"
        # for key,value in p.iteritems():
        #     if key not in self.exclude_list:
        #         msg +=key+":"+str(value)+'\n'
        # self.socket.send( msg )
        # gf = open('pupil_pos', 'a')
        for g in events.get('surface',[]):
            #self.conn.send(str(g['norm_pos']))
            #print g
            try:
                gaze_on_srf = []
                for i in g['gaze_on_srf']:
                    gaze_on_srf.append(i['norm_pos'])

                # strict to surface
                gaze_on_srf = [(x,y) for x,y in gaze_on_srf if x > 0 and x < 1 and y > 0 and y < 1]

                if gaze_on_srf:
                    # calculate center of the gaze coordinates
                    x_avg,y_avg = tuple(map(lambda y: sum(y) / float(len(y)), zip(*gaze_on_srf)))

                    # get distance from each point and the centeroid
                    x_y_distanceFromCenteroid = [(x,y,math.hypot(x - x_avg, y - y_avg)) for x,y in gaze_on_srf]

                    #sort and get closest 75%
                    x_y_distanceFromCenteroid = sorted(x_y_distanceFromCenteroid, key=itemgetter(2))
                    x_y_distanceFromCenteroid = x_y_distanceFromCenteroid[: - int(len(x_y_distanceFromCenteroid)* 0.25 )]

                    # recalculate average without outliers
                    x,y,_ = tuple(map(lambda y: sum(y) / float(len(y)), zip(*x_y_distanceFromCenteroid)))

                    print "sending (" + str(x) + "," + str(y) + ")"
                    self.s.sendto("(" + str(x) + "," + str(y) + ")", (self.ip, self.port))
                    # gf.write(g + '\n')
            except Exception, e:
                print "not on surface:" + str(e)
                traceback.print_exc()
    #for key, value in g.iteritems():
    #self.conn.send( key+":"+str(value)+'\n' )

    #for g in events.get('gaze_positions',[]):
    #   self.conn.send( g['norm_gaze'] )

    #self.conn.send("Hello, World")
    #conn.close()
#[(0.20899205628408357, -0.98328468349377229), (0.21040174508226522, -0.97922400841248292), (0.21048129564469328, -0.98082618370819175), (0.20997948134166367, -0.97962481424446379)]
#0.209963644588 -0.980739922465
#sending (0.209963644588,0)

