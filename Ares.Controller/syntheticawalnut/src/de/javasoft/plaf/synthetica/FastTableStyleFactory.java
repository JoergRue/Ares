package de.javasoft.plaf.synthetica;

import javax.swing.JComponent;
import javax.swing.JTable;
import javax.swing.plaf.synth.Region;
import javax.swing.plaf.synth.SynthStyle;
import javax.swing.plaf.synth.SynthStyleFactory;

import de.javasoft.plaf.synthetica.styles.FastTableStyle;

class FastTableStyleFactory extends StyleFactory {
  
  private SynthStyleFactory myFactory;

  public FastTableStyleFactory(SynthStyleFactory arg0) {
    super(arg0);
    myFactory = arg0;
  }
  
  public synchronized SynthStyle getStyle(JComponent c, Region region) {
    if (region.equals(Region.TABLE)) {
      if (!(c instanceof JTable)) return super.getStyle(c, region);
      JTable table = (JTable) c;
      if (table.getDefaultRenderer(javax.swing.Icon.class) == null) {
        table.setDefaultRenderer(javax.swing.Icon.class, table.getDefaultRenderer(javax.swing.ImageIcon.class));
      }
      return FastTableStyle.getStyle(myFactory.getStyle(c, region), c, region);
    }
    else 
      return super.getStyle(c, region);
  }

}
