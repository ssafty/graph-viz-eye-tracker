import numpy as np
import xml.etree.ElementTree as ET


def calibrate(file_name):
    tree = ET.parse(file_name)
    root = tree.getroot()

    participant = {}
    only_needed_data = {}

    # get the data from all calib rounds to be solved for next step
    for layers in root:
        # print(layers.tag)
        # print(layers.attrib)
        for layer in layers:
            # print(layer.tag)
            # print(layer.attrib)
            layer_key = layer.tag + layer.attrib['layer_id']
            participant[layer_key] = {}
            only_needed_data[layer_key] = {}
            for markers in layer:
                # print(markers.tag)
                # print(markers.attrib)
                for marker in markers:
                    marker_key = marker.tag + marker.attrib['marker_id']
                    participant[layer_key][marker_key] = marker.attrib
                    # print(marker.tag)
                    # print(marker.attrib)
                    for calib_rnds in marker:
                        # print(calib_rnds.tag)
                        # print(calib_rnds.attrib)
                        for calib_rnd in calib_rnds:
                            calib_round_key = calib_rnd.tag + calib_rnd.attrib['calib_round']
                            participant[layer_key][marker_key][calib_round_key] = {}
                            # print(calib_rnd.tag)
                            # print(calib_rnd.attrib)
                            for datas in calib_rnd:
                                participant[layer_key][marker_key][calib_round_key][datas.tag] = []
                                # print(datas.tag)
                                # print(datas.attrib)
                                if datas.tag not in only_needed_data[layer_key]:
                                    only_needed_data[layer_key][datas.tag] = []
                                if 'screen_x' not in only_needed_data[layer_key]:
                                    only_needed_data[layer_key]['screen_x'] = []
                                if 'screen_y' not in only_needed_data[layer_key]:
                                    only_needed_data[layer_key]['screen_y'] = []
                                for val in datas:
                                    participant[layer_key][marker_key][calib_round_key][datas.tag].append(
                                        float(val.text))
                                    # print(val.tag)
                                    # print(val.attrib)
                                    # print(val.text)
                                    only_needed_data[layer_key][datas.tag].append(float(val.text))
                                    if datas.tag == 'left_x':  # one time operation a hack
                                        only_needed_data[layer_key]['screen_x'].append(
                                            float(participant[layer_key][marker_key]['screen_x']))
                                        only_needed_data[layer_key]['screen_y'].append(
                                            float(participant[layer_key][marker_key]['screen_y']))
                                    pass

    # get the calibration coefficeients

    A = [None, None, None]

    for i in range(1):
        layer = only_needed_data['layer' + str(i)]
        # print(layer)
        ip_leftx = np.asarray(layer['left_x'])
        ip_lefty = np.asarray(layer['left_y'])
        ip_leftx2 = ip_leftx ** 2
        ip_lefty2 = ip_lefty ** 2
        ip_ones = np.ones(ip_leftx.shape)
        screen_x = np.asarray(layer['screen_x'])
        screen_y = np.asarray(layer['screen_y'])

        ip = np.asarray([ip_ones, ip_leftx, ip_lefty, ip_leftx2, ip_lefty2])
        op = np.asarray([screen_x, screen_y])

        A[i] = np.dot(np.linalg.pinv(ip).T, op.T)

        op_new = np.dot(ip.T, A[i]).T

        # print(op_new)
        # print(op)

    # store in xml file

    tree = ET.parse(file_name)
    root = tree.getroot()
    for layers in root:
        for layer in layers:
            # print(layer.tag)
            # print(layer.attrib)
            id = int(layer.attrib['layer_id'])
            # print(A[id][0, 0])
            layer.attrib['A0'] = str(A[id][0, 0])
            layer.attrib['A1'] = str(A[id][1, 0])
            layer.attrib['A2'] = str(A[id][2, 0])
            layer.attrib['A3'] = str(A[id][3, 0])
            layer.attrib['A4'] = str(A[id][4, 0])
            layer.attrib['B0'] = str(A[id][0, 1])
            layer.attrib['B1'] = str(A[id][1, 1])
            layer.attrib['B2'] = str(A[id][2, 1])
            layer.attrib['B3'] = str(A[id][3, 1])
            layer.attrib['B4'] = str(A[id][4, 1])

    # tree.write(file_name', encoding="Windows-1252")
    tree.write(file_name)


if __name__ == '__main__':
    import time
    import msvcrt
    import glob
    import os

    while True:

        # exit the code
        if True:
            if msvcrt.kbhit():
                if ord(msvcrt.getch()) == 114:  # r
                    for file_job_done in glob.glob("*.calibjobdone"):
                        os.remove(file_job_done)
                if ord(msvcrt.getch()) == 27:  # ESC
                    print("Quit the job ........")
                    break

        # polling
        time.sleep(1)

        # search the job
        print("Search for job ......")
        for file_job in glob.glob("*.calibjob"):

            if os.path.isfile(file_job.split(".")[0] + ".calibjobdone"):
                continue

            print(">>>> Found job " + file_job)

            file_xml = file_job.split(".")[0] + ".xml"

            if os.path.isfile(file_xml):
                calibrate(file_xml)
                print(">>>> Done calibrating and file written ... " + file_xml)
                file_job_done = open(file_job.split(".")[0] + ".calibjobdone", 'w')
                file_job_done.close()
            else:
                print(
                ">>>> XML file not found. Ignore calibration for " + file_xml + " for corresponding job " + file_job)
                file_job_done = open(file_job.split(".")[0] + ".calibjobdone", 'w')
                file_job_done.close()

        pass



