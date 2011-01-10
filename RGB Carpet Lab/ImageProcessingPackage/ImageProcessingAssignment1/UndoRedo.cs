using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessingAssignment1
{
    public class UndoRedo
    {
        public List<PictureInfo> selectedPic;
        public List<string> done;
        public Stack<string> undo, redo;
        public ListBox undoRedoListBox;
        public int Pointer;
        public UndoRedo(PictureInfo pic, string command)
        {
            selectedPic = new List<PictureInfo>();
            selectedPic.Add(new PictureInfo(pic));
            done = new List<string>();
            done.Add(command);
            undo = new Stack<string>();
            redo = new Stack<string>();
            undoRedoListBox = new ListBox();
            undoRedoListBox.Size = new System.Drawing.Size(256, 270);
            undoRedoListBox.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            undoRedoListBox.ForeColor = System.Drawing.Color.White;
            undoRedoListBox.Items.Add(command);
            undoRedoListBox.SelectedIndex = 0;
            Pointer = 0;
        }
        public void UndoRedoCommands(PictureInfo pic, string command)
        {
            selectedPic.Add(new PictureInfo(pic));
            done.Add(command);
            undo.Push(command);
            undoRedoListBox.Items.Add(command);
            int selected = undoRedoListBox.SelectedIndex;
            int itemsCount = undoRedoListBox.Items.Count - 2;
            int index = itemsCount - selected;
            for (int i = 0; i < index; i++)
            {
                selectedPic.RemoveAt(itemsCount);
                undoRedoListBox.Items.RemoveAt(itemsCount);
                if (undo.Count != 0)
                {
                    undo.Pop();
                    Pointer--;
                }
                done.RemoveAt(itemsCount--);
            }
            undoRedoListBox.SelectedIndex = selected + 1;
            Pointer++;
        }
    }
}