using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessingAssignment1
{
    class UndoRedo
    {
        public PictureInfo selectedPic;
        public List<string> done;
        public Stack<string> undo, redo;
        public ListBox undoRedoListBox;
        public UndoRedo(PictureInfo pic)
        {
            selectedPic = new PictureInfo(pic);
            done = new List<string>();
            done.Add("Open");
            undo = new Stack<string>();
            redo = new Stack<string>();
            undoRedoListBox = new ListBox();
            undoRedoListBox.Size = new System.Drawing.Size(257, 199);
            undoRedoListBox.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            undoRedoListBox.ForeColor = System.Drawing.Color.White;
            undoRedoListBox.Items.Add("Open");
            undoRedoListBox.SelectedIndex = 0;
        }
    }
}