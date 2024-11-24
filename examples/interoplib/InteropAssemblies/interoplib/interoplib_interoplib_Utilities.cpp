//-----------------------------------------------------------------------------
//
//                   ** WARNING! ** 
//    This file was generated automatically by a tool.
//    Re-running the tool will overwrite this file.
//    You should copy this file to a custom location
//    before adding any customization in the copy to
//    prevent loss of your changes when the tool is
//    re-run.
//
//-----------------------------------------------------------------------------

#include "interoplib.h"
#include "interoplib_interoplib_Utilities.h"

using namespace interoplib::interoplib;


void Utilities::NativeGetHardwareSerial( CLR_RT_TypedArray_UINT8 param0, HRESULT &hr )
{
    memcpy((void*)param0.GetBuffer(), (const void*)0x1FFF7A10, 12);
}