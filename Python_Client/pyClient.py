import os, sys
import io

def start():
    try:
        #linux
        receiver = int(sys.argv[1])
        sender = int(sys.argv[2])

        writeHandle = readHandle = None

        if(os.name == 'nt'):
            #windows
            import msvcrt
            readHandle = msvcrt.open_osfhandle(receiver, os.O_RDONLY)
            writeHandle = msvcrt.open_osfhandle(sender, os.O_WRONLY)

        print('os '+ os.name+' detected')
        reader = os.fdopen(readHandle or receiver,'rb')
        writer = os.fdopen(writeHandle or sender, 'wb')

        readObj = io.BufferedReader(reader.raw)
        writeObj = io.BufferedWriter(writer.raw)

        syncreceived = False
        count =1 
        while not syncreceived:
            if(readObj.readline().startswith(str.encode('SYNC','utf-8'),0)):
                print('SYNC received from server')
                syncreceived = True
                break
            else:
                print('waiting for SYNC')
        while True:
            #read
            input = readObj.readline()

            if input:
                print('Python client has received >>')
                print(input)

                print('Python client has sent >>')
                #write
                writeObj.write(bytes('python client sent msg '+ str(count),'utf-8'))
                writeObj.write(bytes('\n','utf-8'))
                writeObj.flush()
                count+=1

    except Exception as e:
        print(e)


if __name__ == '__main__':
    print('Initiating Python client')

    if (not len(sys.argv))<3:
        start()
    else:
        print('Insuffient Arguments passed')
