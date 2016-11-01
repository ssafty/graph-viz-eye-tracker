from plugin import Plugin
import socket
import os
from pyglui import ui

# This file has to be copied to pupil/capture_settings/plugins/

class Udp_Server(Plugin):
    """
    Udp Server broadcasts the gaze position
    over the network
    """

    def __init__(self, g_pool):
        super(Udp_Server, self).__init__(g_pool)
        self.order = .9
        self.ip = '192.168.0.2' #change to unity IP
        self.port = 5007        #change to unity port
        self.s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def init_gui(self):
        if self.g_pool.app == 'capture':
            self.menu = ui.Growing_Menu("UDP Broadcast Server")
            self.g_pool.sidebar.append(self.menu)

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
                # print g['gaze_on_srf']
                gaze_on_srf = []
                for i in g['gaze_on_srf']:
                    gaze_on_srf.append(i['norm_pos']) 
                
                # taking average of points in gaze_on_srf on a given event
                # TODO research incremental averaging to avoid computation speed issues (yet functional lambda it should be fast enough)
                if gaze_on_srf:
                    x,y = tuple(map(lambda y: sum(y) / float(len(y)), zip(*gaze_on_srf)))
                
                    # use the for loop if every single coordinate should be used
                    # for x,y in g['gaze_on_srf']:
                    print x,y
                
                    # bound the x,y between 0 and 1 to get coordinates on the screen
                    # TODO fix the "Show Calibration" to understand the uncertainty areas more.
                    # TODO 2 If you managed to fix "Show Calibration", make a Pull Request to Pupil Labs -_-" 
                    lo,hi = 0,1
                    x = lo if x <= lo else hi if x>=hi else x
                    y = lo if y <= lo else hi if y>=hi else y

                    print "(" + str(x) + "," + str(y) + ")"
                    self.s.sendto((" + str(x) + "," + str(y) + "), (self.ip, self.port))
                    # gf.write(g + '\n')
            except Exception, e:
                print "not on surface:" + str(e)
    #for key, value in g.iteritems():
    #self.conn.send( key+":"+str(value)+'\n' )

    #for g in events.get('gaze_positions',[]):
    #   self.conn.send( g['norm_gaze'] )

    #self.conn.send("Hello, World")
    #conn.close()

