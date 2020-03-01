import os,sys,io
import pdb


def start():
    try:
        print(sys.argv[1])
        print(sys.argv[2])
        syncreceived = False
        count =1 
        while not syncreceived:
            if(sys.argv[1]=='SYNC'):
                print('SYNC received from server')
                syncreceived = True
                break
            else:
                print('waiting for SYNC')
        while True:
            input = sys.argv[2]
            if input:
                print('Python client has received >>')
                #pdb.set_trace()
                print(input)

                print('Python client has sent >>')
                print(bytes('python sent msg','utf-8'))
                print(bytes('\n','utf-8'))
                break
                #pdb.set_trace()
                count+=1
                
    except Exception as e:
        print(e)

if __name__ == '__main__':
    print('Initiating Python client')

    if (not len(sys.argv))<3:
        print(len(sys.argv))
        start()
    else:
        print('Insuffient Arguments passed')
